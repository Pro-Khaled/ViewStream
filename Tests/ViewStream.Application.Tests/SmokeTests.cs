using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ViewStream.Application.Commands.Notification.CreateNotification;
using ViewStream.Application.DTOs;
using ViewStream.Application.Contracts;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Tests
{
    public class SmokeTests
    {
        [Fact]
        public async Task CreateNotificationCommand_Should_SendInAppAndPublishMessage()
        {
            // Arrange
            var notificationRepoMock = new Mock<INotificationRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.Notifications).Returns(notificationRepoMock.Object);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Notification>(It.IsAny<CreateNotificationDto>()))
                .Returns((CreateNotificationDto src) => new Notification
                {
                    UserId = src.UserId,
                    Title = src.Title,
                    Body = src.Body,
                    NotificationType = src.NotificationType,
                    Channel = src.Channel
                });

            mapperMock.Setup(m => m.Map<NotificationDto>(It.IsAny<Notification>()))
                .Returns((Notification src) => new NotificationDto
                {
                    Id = src.Id,
                    UserId = src.UserId,
                    Title = src.Title,
                    Body = src.Body,
                    NotificationType = src.NotificationType
                });

            var auditContextMock = new Mock<IAuditContext>();
            
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var messageBusMock = new Mock<IMessageBus>();
            var inAppSenderMock = new Mock<IInAppNotificationSender>();
            var loggerMock = new Mock<ILogger<CreateNotificationCommandHandler>>();

            var handler = new CreateNotificationCommandHandler(
                unitOfWorkMock.Object,
                mapperMock.Object,
                auditContextMock.Object,
                userManagerMock.Object,
                messageBusMock.Object,
                inAppSenderMock.Object,
                loggerMock.Object
            );

            var dto = new CreateNotificationDto
            {
                UserId = 42,
                Title = "Test Notification",
                Body = "This is a smoke test",
                NotificationType = "System",
                Channel = "All"
            };

            var command = new CreateNotificationCommand(dto, ActorUserId: 1);

            // Mock database insertion to assign an ID
            notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Callback<Notification, CancellationToken>((n, c) => n.Id = 999)
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(999);
            result.UserId.Should().Be(42);

            // Verify Repository Add and SaveChanges were called
            notificationRepoMock.Verify(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify in-app sending was invoked
            inAppSenderMock.Verify(s => s.SendToUserAsync(42, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);

            // Verify message bus published background job
            messageBusMock.Verify(m => m.Publish(It.Is<SendNotificationMessage>(msg => msg.NotificationId == 999)), Times.Once);

            // Verify audit context properties were set
            auditContextMock.VerifySet(a => a.TableName = "Notifications", Times.Once);
            auditContextMock.VerifySet(a => a.RecordId = 999, Times.Once);
            auditContextMock.VerifySet(a => a.Action = "INSERT", Times.Once);
            auditContextMock.VerifySet(a => a.ChangedByUserId = 1, Times.Once);
        }
    }
}