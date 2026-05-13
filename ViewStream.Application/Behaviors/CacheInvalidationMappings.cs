using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Commands.AudioTrack.CreateAudioTrack;
using ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack;
using ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack;
using ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack;
using ViewStream.Application.Commands.Award.CreateAward;
using ViewStream.Application.Commands.Award.DeleteAward;
using ViewStream.Application.Commands.Award.UpdateAward;
using ViewStream.Application.Commands.CommentLike.CreateCommentLike;
using ViewStream.Application.Commands.CommentLike.DeleteCommentLike;
using ViewStream.Application.Commands.CommentReport.CreateCommentReport;
using ViewStream.Application.Commands.CommentReport.UpdateCommentReport;
using ViewStream.Application.Commands.ContentReport.CreateContentReport;
using ViewStream.Application.Commands.ContentReport.UpdateContentReport;
using ViewStream.Application.Commands.ContentTag.CreateContentTag;
using ViewStream.Application.Commands.ContentTag.DeleteContentTag;
using ViewStream.Application.Commands.ContentTag.UpdateContentTag;
using ViewStream.Application.Commands.Country.CreateCountry;
using ViewStream.Application.Commands.Country.DeleteCountry;
using ViewStream.Application.Commands.Country.UpdateCountry;
using ViewStream.Application.Commands.Credit.CreateCredit;
using ViewStream.Application.Commands.Credit.DeleteCredit;
using ViewStream.Application.Commands.Credit.UpdateCredit;
using ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest;
using ViewStream.Application.Commands.DataDeletionRequest.DeleteDataDeletionRequest;
using ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest;
using ViewStream.Application.Commands.Device.CreateDevice;
using ViewStream.Application.Commands.Device.DeleteDevice;
using ViewStream.Application.Commands.Device.UpdateDevice;
using ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference;
using ViewStream.Application.Commands.Episode.CreateEpisode;
using ViewStream.Application.Commands.Episode.DeleteEpisode;
using ViewStream.Application.Commands.Episode.RestoreEpisode;
using ViewStream.Application.Commands.Episode.UpdateEpisode;
using ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment;
using ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment;
using ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment;
using ViewStream.Application.Commands.Friendship.RespondToFriendRequest;
using ViewStream.Application.Commands.Friendship.SendFriendRequest;
using ViewStream.Application.Commands.Friendship.Unfriend;
using ViewStream.Application.Commands.Genre.CreateGenre;
using ViewStream.Application.Commands.Genre.DeleteGenre;
using ViewStream.Application.Commands.Genre.UpdateGenre;
using ViewStream.Application.Commands.ItemVector.UpsertItemVector;
using ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions;
using ViewStream.Application.Commands.LoginSession.RevokeLoginSession;
using ViewStream.Application.Commands.Notification.CreateNotification;
using ViewStream.Application.Commands.Notification.DeleteNotification;
using ViewStream.Application.Commands.Notification.MarkAllNotificationsAsRead;
using ViewStream.Application.Commands.Notification.MarkNotificationAsRead;
using ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload;
using ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload;
using ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod;
using ViewStream.Application.Commands.Permission.CreatePermission;
using ViewStream.Application.Commands.Permission.DeletePermission;
using ViewStream.Application.Commands.Permission.UpdatePermission;
using ViewStream.Application.Commands.Person.CreatePerson;
using ViewStream.Application.Commands.Person.DeletePerson;
using ViewStream.Application.Commands.Person.UpdatePerson;
using ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow;
using ViewStream.Application.Commands.PersonalizedRow.RegenerateRecommendations;
using ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow;
using ViewStream.Application.Commands.PersonAward.AddPersonAward;
using ViewStream.Application.Commands.PersonAward.RemovePersonAward;
using ViewStream.Application.Commands.Profile.CreateProfile;
using ViewStream.Application.Commands.Profile.DeleteProfile;
using ViewStream.Application.Commands.Profile.UpdateProfile;
using ViewStream.Application.Commands.PromoCode.CreatePromoCode;
using ViewStream.Application.Commands.PromoCode.DeletePromoCode;
using ViewStream.Application.Commands.PromoCode.UpdatePromoCode;
using ViewStream.Application.Commands.PushToken.DeletePushToken;
using ViewStream.Application.Commands.PushToken.RegisterPushToken;
using ViewStream.Application.Commands.Rating.CreateRating;
using ViewStream.Application.Commands.Rating.DeleteRating;
using ViewStream.Application.Commands.Role.CreateRole;
using ViewStream.Application.Commands.Role.DeleteRole;
using ViewStream.Application.Commands.Role.UpdateRole;
using ViewStream.Application.Commands.Season.CreateSeason;
using ViewStream.Application.Commands.Season.DeleteSeason;
using ViewStream.Application.Commands.Season.RestoreSeason;
using ViewStream.Application.Commands.Season.UpdateSeason;
using ViewStream.Application.Commands.SharedList.CreateSharedList;
using ViewStream.Application.Commands.SharedList.DeleteSharedList;
using ViewStream.Application.Commands.SharedList.UpdateSharedList;
using ViewStream.Application.Commands.SharedListItem.AddShowToSharedList;
using ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList;
using ViewStream.Application.Commands.Show.CreateShow;
using ViewStream.Application.Commands.Show.DeleteShow;
using ViewStream.Application.Commands.Show.RestoreShow;
using ViewStream.Application.Commands.Show.UpdateShow;
using ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability;
using ViewStream.Application.Commands.ShowAward.AddShowAward;
using ViewStream.Application.Commands.ShowAward.RemoveShowAward;
using ViewStream.Application.Commands.Subscription.CreateSubscription;
using ViewStream.Application.Commands.Subscription.DeleteSubscription;
using ViewStream.Application.Commands.Subscription.UpdateSubscription;
using ViewStream.Application.Commands.Subtitle.CreateSubtitle;
using ViewStream.Application.Commands.Subtitle.DeleteSubtitle;
using ViewStream.Application.Commands.Subtitle.RestoreSubtitle;
using ViewStream.Application.Commands.Subtitle.UpdateSubtitle;
using ViewStream.Application.Commands.User.AdminUpdateUser;
using ViewStream.Application.Commands.User.DeleteUser;
using ViewStream.Application.Commands.User.UnblockUser;
using ViewStream.Application.Commands.UserInteraction.CreateUserInteraction;
using ViewStream.Application.Commands.UserLibrary.CreateUserLibrary;
using ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary;
using ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary;
using ViewStream.Application.Commands.UserRole.AssignRoleToUser;
using ViewStream.Application.Commands.UserRole.RemoveRoleFromUser;
using ViewStream.Application.Commands.UserVector.UpsertUserVector;
using ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory;
using ViewStream.Application.Commands.WatchParty.CreateWatchParty;
using ViewStream.Application.Commands.WatchParty.DeleteWatchParty;
using ViewStream.Application.Commands.WatchParty.UpdateWatchParty;
using ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty;
using ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty;

namespace ViewStream.Application.Behaviors
{
    public static class CacheInvalidationMappings
    {
        public static IReadOnlyDictionary<Type, string[]> Mappings { get; } = new Dictionary<Type, string[]>
        {
                        // ==================== SHOW ====================
            { typeof(CreateShowCommand), new[] { "GetShowsPagedQuery_*", "GetAdminShowsPagedQuery_*" } },
            { typeof(UpdateShowCommand), new[]
                {
                    "GetShowsPagedQuery_*",
                    "GetShowByIdQuery_*{Id}*",
                    "GetAdminShowsPagedQuery_*",
                    "GetGenresPagedQuery_*",          // because show may change genres
                    "GetAllGenresQuery_*",
                    "GetContentTagsPagedQuery_*",      // show tags might change
                    "GetAllContentTagsQuery_*",
                    "GetActiveWatchPartiesPagedQuery_*" // trailer updates affect watch parties
                }
            },
            { typeof(DeleteShowCommand), new[]
                {
                    "GetShowsPagedQuery_*",
                    "GetShowByIdQuery_*{Id}*",
                    "GetAdminShowsPagedQuery_*",
                    "GetSeasonsByShowQuery_*",
                    "GetEpisodesBySeasonQuery_*",
                    "GetActiveWatchPartiesPagedQuery_*"
                }
            },
            { typeof(RestoreShowCommand), new[]
                {
                    "GetShowsPagedQuery_*",
                    "GetShowByIdQuery_*{Id}*",
                    "GetAdminShowsPagedQuery_*",
                    "GetSeasonsByShowQuery_*"
                }
            },

            // ==================== SEASON ====================
            { typeof(CreateSeasonCommand), new[]
                {
                    "GetShowsPagedQuery_*",
                    "GetSeasonsByShowQuery_*",
                    "GetAdminSeasonsPagedQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },
            { typeof(UpdateSeasonCommand), new[]
                {
                    "GetSeasonByIdQuery_*{Id}*",
                    "GetSeasonsByShowQuery_*",
                    "GetAdminSeasonsPagedQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },
            { typeof(DeleteSeasonCommand), new[]
                {
                    "GetSeasonByIdQuery_*{Id}*",
                    "GetSeasonsByShowQuery_*",
                    "GetAdminSeasonsPagedQuery_*",
                    "GetShowByIdQuery_*{ShowId}*",
                    "GetEpisodesBySeasonQuery_*"
                }
            },
            { typeof(RestoreSeasonCommand), new[]
                {
                    "GetSeasonByIdQuery_*{Id}*",
                    "GetSeasonsByShowQuery_*",
                    "GetAdminSeasonsPagedQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },

            // ==================== EPISODE ====================
            { typeof(CreateEpisodeCommand), new[]
                {
                    "GetEpisodesBySeasonQuery_*",
                    "GetSeasonsByShowQuery_*",
                    "GetAdminEpisodesPagedQuery_*",
                    "GetSeasonByIdQuery_*{SeasonId}*",
                    "GetShowByIdQuery_*{ShowId}*",
                    "GetAudioTracksByEpisodeQuery_*",
                    "GetSubtitlesByEpisodeQuery_*"
                }
            },
            { typeof(UpdateEpisodeCommand), new[]
                {
                    "GetEpisodeByIdQuery_*{Id}*",
                    "GetEpisodesBySeasonQuery_*",
                    "GetAdminEpisodesPagedQuery_*",
                    "GetAudioTracksByEpisodeQuery_*",
                    "GetSubtitlesByEpisodeQuery_*"
                }
            },
            { typeof(DeleteEpisodeCommand), new[]
                {
                    "GetEpisodeByIdQuery_*{Id}*",
                    "GetEpisodesBySeasonQuery_*",
                    "GetAdminEpisodesPagedQuery_*",
                    "GetAudioTracksByEpisodeQuery_*",
                    "GetSubtitlesByEpisodeQuery_*",
                    "GetActiveWatchPartiesPagedQuery_*",
                    "GetWatchPartyByIdQuery_*",
                    "GetWatchPartyByCodeQuery_*"
                }
            },
            { typeof(RestoreEpisodeCommand), new[]
                {
                    "GetEpisodeByIdQuery_*{Id}*",
                    "GetEpisodesBySeasonQuery_*",
                    "GetAdminEpisodesPagedQuery_*"
                }
            },

            // ==================== GENRE ====================
            { typeof(CreateGenreCommand), new[] { "GetGenresPagedQuery_*", "GetAllGenresQuery_*", "GetAdminGenresPagedQuery_*" } },
            { typeof(UpdateGenreCommand), new[]
                {
                    "GetGenreByIdQuery_*{Id}*",
                    "GetGenresPagedQuery_*",
                    "GetAllGenresQuery_*",
                    "GetAdminGenresPagedQuery_*",
                    "GetShowsPagedQuery_*"        // because shows have genres
                }
            },
            { typeof(DeleteGenreCommand), new[]
                {
                    "GetGenreByIdQuery_*{Id}*",
                    "GetGenresPagedQuery_*",
                    "GetAllGenresQuery_*",
                    "GetAdminGenresPagedQuery_*",
                    "GetShowsPagedQuery_*"
                }
            },

            // ==================== COUNTRY ====================
            { typeof(CreateCountryCommand), new[] { "GetCountriesPagedQuery_*", "GetAllCountriesQuery_*", "GetAdminCountriesPagedQuery_*" } },
            { typeof(UpdateCountryCommand), new[]
                {
                    "GetCountryByCodeQuery_*{Code}*",
                    "GetCountriesPagedQuery_*",
                    "GetAllCountriesQuery_*",
                    "GetAdminCountriesPagedQuery_*",
                    "GetAvailabilitiesByCountryQuery_*"
                }
            },
            { typeof(DeleteCountryCommand), new[]
                {
                    "GetCountryByCodeQuery_*{Code}*",
                    "GetCountriesPagedQuery_*",
                    "GetAllCountriesQuery_*",
                    "GetAdminCountriesPagedQuery_*",
                    "GetAvailabilitiesByCountryQuery_*",
                    "GetAvailabilitiesByShowQuery_*"
                }
            },

            // ==================== PERSON ====================
            { typeof(CreatePersonCommand), new[] { "GetPersonsPagedQuery_*", "GetAllPersonsQuery_*", "GetAdminPersonsPagedQuery_*" } },
            { typeof(UpdatePersonCommand), new[]
                {
                    "GetPersonByIdQuery_*{Id}*",
                    "GetPersonsPagedQuery_*",
                    "GetAllPersonsQuery_*",
                    "GetAdminPersonsPagedQuery_*",
                    "GetCreditsByPersonQuery_*"
                }
            },
            { typeof(DeletePersonCommand), new[]
                {
                    "GetPersonByIdQuery_*{Id}*",
                    "GetPersonsPagedQuery_*",
                    "GetAllPersonsQuery_*",
                    "GetAdminPersonsPagedQuery_*",
                    "GetCreditsByPersonQuery_*"
                }
            },

            // ==================== CREDIT ====================
            { typeof(CreateCreditCommand), new[]
                {
                    "GetCreditsPagedQuery_*",
                    "GetAdminCreditsPagedQuery_*",
                    "GetCreditsByShowQuery_*",
                    "GetCreditsBySeasonQuery_*",
                    "GetCreditsByEpisodeQuery_*",
                    "GetCreditsByPersonQuery_*",
                    "GetShowByIdQuery_*{ShowId}*",
                    "GetSeasonByIdQuery_*{SeasonId}*",
                    "GetEpisodeByIdQuery_*{EpisodeId}*"
                }
            },
            { typeof(UpdateCreditCommand), new[]
                {
                    "GetCreditByIdQuery_*{Id}*",
                    "GetCreditsPagedQuery_*",
                    "GetAdminCreditsPagedQuery_*",
                    "GetCreditsByShowQuery_*",
                    "GetCreditsBySeasonQuery_*",
                    "GetCreditsByEpisodeQuery_*",
                    "GetCreditsByPersonQuery_*"
                }
            },
            { typeof(DeleteCreditCommand), new[]
                {
                    "GetCreditByIdQuery_*{Id}*",
                    "GetCreditsPagedQuery_*",
                    "GetAdminCreditsPagedQuery_*",
                    "GetCreditsByShowQuery_*",
                    "GetCreditsBySeasonQuery_*",
                    "GetCreditsByEpisodeQuery_*",
                    "GetCreditsByPersonQuery_*"
                }
            },

            // ==================== AWARD ====================
            { typeof(CreateAwardCommand), new[] { "GetAwardsPagedQuery_*", "GetAllAwardsQuery_*", "GetAdminAwardsPagedQuery_*" } },
            { typeof(UpdateAwardCommand), new[]
                {
                    "GetAwardByIdQuery_*{Id}*",
                    "GetAwardsPagedQuery_*",
                    "GetAllAwardsQuery_*",
                    "GetAdminAwardsPagedQuery_*",
                    "GetPersonAwardsQuery_*",
                    "GetShowAwardsQuery_*"
                }
            },
            { typeof(DeleteAwardCommand), new[]
                {
                    "GetAwardByIdQuery_*{Id}*",
                    "GetAwardsPagedQuery_*",
                    "GetAllAwardsQuery_*",
                    "GetAdminAwardsPagedQuery_*",
                    "GetPersonAwardsQuery_*",
                    "GetShowAwardsQuery_*"
                }
            },

            // ==================== CONTENT TAG ====================
            { typeof(CreateContentTagCommand), new[] { "GetContentTagsPagedQuery_*", "GetAllContentTagsQuery_*", "GetAdminContentTagsPagedQuery_*" } },
            { typeof(UpdateContentTagCommand), new[]
                {
                    "GetContentTagByIdQuery_*{Id}*",
                    "GetContentTagsPagedQuery_*",
                    "GetAllContentTagsQuery_*",
                    "GetAdminContentTagsPagedQuery_*",
                    "GetShowsPagedQuery_*"
                }
            },
            { typeof(DeleteContentTagCommand), new[]
                {
                    "GetContentTagByIdQuery_*{Id}*",
                    "GetContentTagsPagedQuery_*",
                    "GetAllContentTagsQuery_*",
                    "GetAdminContentTagsPagedQuery_*",
                    "GetShowsPagedQuery_*"
                }
            },

            // ==================== AUDIO TRACK ====================
            { typeof(CreateAudioTrackCommand), new[] { "GetAudioTracksByEpisodeQuery_*", "GetAdminAudioTracksPagedQuery_*", "GetEpisodeByIdQuery_*{EpisodeId}*" } },
            { typeof(UpdateAudioTrackCommand), new[]
                {
                    "GetAudioTrackByIdQuery_*{Id}*",
                    "GetAudioTracksByEpisodeQuery_*",
                    "GetAdminAudioTracksPagedQuery_*",
                    "GetEpisodeByIdQuery_*{EpisodeId}*"
                }
            },
            { typeof(DeleteAudioTrackCommand), new[]
                {
                    "GetAudioTrackByIdQuery_*{Id}*",
                    "GetAudioTracksByEpisodeQuery_*",
                    "GetAdminAudioTracksPagedQuery_*",
                    "GetEpisodeByIdQuery_*{EpisodeId}*"
                }
            },
            { typeof(RestoreAudioTrackCommand), new[] { "GetAudioTracksByEpisodeQuery_*", "GetAdminAudioTracksPagedQuery_*" } },

            // ==================== SUBTITLE ====================
            { typeof(CreateSubtitleCommand), new[] { "GetSubtitlesByEpisodeQuery_*", "GetAdminSubtitlesPagedQuery_*", "GetEpisodeByIdQuery_*{EpisodeId}*" } },
            { typeof(UpdateSubtitleCommand), new[]
                {
                    "GetSubtitleByIdQuery_*{Id}*",
                    "GetSubtitlesByEpisodeQuery_*",
                    "GetAdminSubtitlesPagedQuery_*",
                    "GetEpisodeByIdQuery_*{EpisodeId}*"
                }
            },
            { typeof(DeleteSubtitleCommand), new[]
                {
                    "GetSubtitleByIdQuery_*{Id}*",
                    "GetSubtitlesByEpisodeQuery_*",
                    "GetAdminSubtitlesPagedQuery_*",
                    "GetEpisodeByIdQuery_*{EpisodeId}*"
                }
            },
            { typeof(RestoreSubtitleCommand), new[] { "GetSubtitlesByEpisodeQuery_*", "GetAdminSubtitlesPagedQuery_*" } },

            // ==================== SHOW AVAILABILITY ====================
            { typeof(CreateShowAvailabilityCommand), new[]
                {
                    "GetAvailabilitiesByShowQuery_*",
                    "GetAvailabilitiesByCountryQuery_*",
                    "GetShowAvailabilityQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },
            { typeof(UpdateShowAvailabilityCommand), new[]
                {
                    "GetAvailabilitiesByShowQuery_*",
                    "GetAvailabilitiesByCountryQuery_*",
                    "GetShowAvailabilityQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },
            { typeof(DeleteShowAvailabilityCommand), new[]
                {
                    "GetAvailabilitiesByShowQuery_*",
                    "GetAvailabilitiesByCountryQuery_*",
                    "GetShowAvailabilityQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },

            // ==================== PERSON AWARD ====================
            { typeof(AddPersonAwardCommand), new[] { "GetPersonAwardsQuery_*", "GetPersonByIdQuery_*{PersonId}*" } },
            { typeof(RemovePersonAwardCommand), new[] { "GetPersonAwardsQuery_*", "GetPersonByIdQuery_*{PersonId}*" } },

            // ==================== SHOW AWARD ====================
            { typeof(AddShowAwardCommand), new[] { "GetShowAwardsQuery_*", "GetShowByIdQuery_*{ShowId}*" } },
            { typeof(RemoveShowAwardCommand), new[] { "GetShowAwardsQuery_*", "GetShowByIdQuery_*{ShowId}*" } },

            // ==================== PROMO CODE ====================
            { typeof(CreatePromoCodeCommand), new[] { "GetPromoCodesPagedQuery_*", "GetAdminPromoCodesPagedQuery_*" } },
            { typeof(UpdatePromoCodeCommand), new[]
                {
                    "GetPromoCodeByIdQuery_*{Id}*",
                    "GetPromoCodesPagedQuery_*",
                    "GetAdminPromoCodesPagedQuery_*",
                    "GetPromoCodeByCodeQuery_*"
                }
            },
            { typeof(DeletePromoCodeCommand), new[]
                {
                    "GetPromoCodeByIdQuery_*{Id}*",
                    "GetPromoCodesPagedQuery_*",
                    "GetAdminPromoCodesPagedQuery_*",
                    "GetPromoCodeByCodeQuery_*"
                }
            },

            // ==================== SHARED LIST ====================
            { typeof(CreateSharedListCommand), new[]
                {
                    "GetSharedListsByProfileQuery_*",
                    "GetPublicSharedListsPagedQuery_*",
                    "GetSharedListByIdQuery_*"
                }
            },
            { typeof(UpdateSharedListCommand), new[]
                {
                    "GetSharedListByIdQuery_*{Id}*",
                    "GetSharedListsByProfileQuery_*",
                    "GetPublicSharedListsPagedQuery_*",
                    "GetSharedListByShareCodeQuery_*"
                }
            },
            { typeof(DeleteSharedListCommand), new[]
                {
                    "GetSharedListByIdQuery_*{Id}*",
                    "GetSharedListsByProfileQuery_*",
                    "GetPublicSharedListsPagedQuery_*",
                    "GetSharedListByShareCodeQuery_*"
                }
            },

            // ==================== SHARED LIST ITEM ====================
            { typeof(AddShowToSharedListCommand), new[]
                {
                    "GetItemsBySharedListQuery_*",
                    "GetSharedListByIdQuery_*{ListId}*",
                    "GetSharedListsByProfileQuery_*"
                }
            },
            { typeof(RemoveShowFromSharedListCommand), new[]
                {
                    "GetItemsBySharedListQuery_*",
                    "GetSharedListByIdQuery_*{ListId}*",
                    "GetSharedListsByProfileQuery_*"
                }
            },

            // ==================== USER ====================
            { typeof(AdminUpdateUserCommand), new[]
                {
                    "GetAdminUsersPagedQuery_*",
                    "GetUserByIdQuery_*{UserId}*",
                    "GetCurrentUserQuery_*",
                    "SearchUsersQuery_*"
                }
            },
            { typeof(Commands.User.BlockUser.BlockUserCommand), new[] { "GetAdminUsersPagedQuery_*", "GetUserByIdQuery_*{UserId}*", "SearchUsersQuery_*" } },
            { typeof(UnblockUserCommand), new[] { "GetAdminUsersPagedQuery_*", "GetUserByIdQuery_*{UserId}*", "SearchUsersQuery_*" } },
            { typeof(DeleteUserCommand), new[] { "GetAdminUsersPagedQuery_*", "GetUserByIdQuery_*{UserId}*", "SearchUsersQuery_*" } },

            // ==================== USER ROLE ====================
            { typeof(AssignRoleToUserCommand), new[]
                {
                    "GetUserRolesQuery_*",
                    "GetUserByIdQuery_*{UserId}*",
                    "GetAdminUsersPagedQuery_*",
                    "GetCurrentUserQuery_*"
                }
            },
            { typeof(RemoveRoleFromUserCommand), new[]
                {
                    "GetUserRolesQuery_*",
                    "GetUserByIdQuery_*{UserId}*",
                    "GetAdminUsersPagedQuery_*",
                    "GetCurrentUserQuery_*"
                }
            },

            // ==================== PROFILE ====================
            { typeof(CreateProfileCommand), new[]
                {
                    "GetProfilesByUserQuery_*",
                    "GetProfileByIdQuery_*",
                    "GetCurrentUserQuery_*"
                }
            },
            { typeof(UpdateProfileCommand), new[]
                {
                    "GetProfileByIdQuery_*{Id}*",
                    "GetProfilesByUserQuery_*",
                    "GetCurrentUserQuery_*",
                    "GetPersonalizedRowsByProfileQuery_*",
                    "GetUserLibrarySummaryQuery_*",
                    "GetUserLibraryPagedQuery_*"
                }
            },
            { typeof(DeleteProfileCommand), new[]
                {
                    "GetProfileByIdQuery_*{Id}*",
                    "GetProfilesByUserQuery_*",
                    "GetCurrentUserQuery_*",
                    "GetPersonalizedRowsByProfileQuery_*",
                    "GetUserLibrarySummaryQuery_*"
                }
            },

            // ==================== USER INTERACTION ====================
            { typeof(CreateUserInteractionCommand), new[]
                {
                    "GetUserInteractionsPagedQuery_*",
                    "GetProfileInteractionSummaryQuery_*",
                    "GetAdminUserInteractionsPagedQuery_*"
                }
            },

            // ==================== USER LIBRARY ====================
            { typeof(CreateUserLibraryCommand), new[]
                {
                    "GetUserLibraryPagedQuery_*",
                    "GetUserLibrarySummaryQuery_*",
                    "GetUserLibraryByTargetQuery_*"
                }
            },
            { typeof(UpdateUserLibraryCommand), new[]
                {
                    "GetUserLibraryByIdQuery_*{Id}*",
                    "GetUserLibraryPagedQuery_*",
                    "GetUserLibrarySummaryQuery_*",
                    "GetUserLibraryByTargetQuery_*"
                }
            },
            { typeof(DeleteUserLibraryCommand), new[]
                {
                    "GetUserLibraryByIdQuery_*{Id}*",
                    "GetUserLibraryPagedQuery_*",
                    "GetUserLibrarySummaryQuery_*",
                    "GetUserLibraryByTargetQuery_*"
                }
            },

            // ==================== WATCH HISTORY ====================
            { typeof(UpsertWatchHistoryCommand), new[]
                {
                    "GetWatchHistoryPagedQuery_*",
                    "GetContinueWatchingQuery_*",
                    "GetUserLibraryPagedQuery_*"        // may affect "watching" status
                }
            },

            // ==================== RATING ====================
            { typeof(UpsertRatingCommand), new[]
                {
                    "GetRatingsByShowQuery_*",
                    "GetShowRatingSummaryQuery_*",
                    "GetUserRatingForShowQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },
            { typeof(DeleteRatingCommand), new[]
                {
                    "GetRatingsByShowQuery_*",
                    "GetShowRatingSummaryQuery_*",
                    "GetUserRatingForShowQuery_*",
                    "GetShowByIdQuery_*{ShowId}*"
                }
            },

            // ==================== COMMENT ====================
            { typeof(CreateEpisodeCommentCommand), new[]
                {
                    "GetRootCommentsPagedQuery_*",
                    "GetRepliesByParentQuery_*",
                    "GetCommentWithRepliesQuery_*"
                }
            },
            { typeof(UpdateEpisodeCommentCommand), new[]
                {
                    "GetCommentWithRepliesQuery_*{Id}*",
                    "GetRootCommentsPagedQuery_*",
                    "GetRepliesByParentQuery_*"
                }
            },
            { typeof(DeleteEpisodeCommentCommand), new[]
                {
                    "GetCommentWithRepliesQuery_*{Id}*",
                    "GetRootCommentsPagedQuery_*",
                    "GetRepliesByParentQuery_*",
                    "GetCommentReactionSummaryQuery_*"
                }
            },

            // ==================== COMMENT LIKE ====================
            { typeof(UpsertCommentLikeCommand), new[]
                {
                    "GetCommentReactionSummaryQuery_*",
                    "GetReactionsByCommentQuery_*",
                    "GetUserReactionForCommentQuery_*"
                }
            },
            { typeof(DeleteCommentLikeCommand), new[]
                {
                    "GetCommentReactionSummaryQuery_*",
                    "GetReactionsByCommentQuery_*",
                    "GetUserReactionForCommentQuery_*"
                }
            },

            // ==================== COMMENT REPORT ====================
            { typeof(CreateCommentReportCommand), new[] { "GetAdminCommentReportsPagedQuery_*", "GetReportByIdQuery_*" } },
            { typeof(UpdateReportStatusCommand), new[]
                {
                    "GetReportByIdQuery_*{ReportId}*",
                    "GetAdminCommentReportsPagedQuery_*",
                    "GetReportsPagedQuery_*"
                }
            },

            // ==================== CONTENT REPORT ====================
            { typeof(CreateContentReportCommand), new[] { "GetAdminContentReportsPagedQuery_*", "GetContentReportByIdQuery_*" } },
            { typeof(UpdateContentReportStatusCommand), new[]
                {
                    "GetContentReportByIdQuery_*{ReportId}*",
                    "GetAdminContentReportsPagedQuery_*",
                    "GetContentReportsPagedQuery_*"
                }
            },

            // ==================== DATA DELETION REQUEST ====================
            { typeof(CreateDataDeletionRequestCommand), new[] { "GetAdminDataDeletionRequestsPagedQuery_*", "GetDataDeletionRequestByIdQuery_*" } },
            { typeof(UpdateDataDeletionRequestCommand), new[]
                {
                    "GetDataDeletionRequestByIdQuery_*{Id}*",
                    "GetAdminDataDeletionRequestsPagedQuery_*",
                    "GetDataDeletionRequestsPagedQuery_*"
                }
            },
            { typeof(DeleteDataDeletionRequestCommand), new[]
                {
                    "GetDataDeletionRequestByIdQuery_*{Id}*",
                    "GetAdminDataDeletionRequestsPagedQuery_*",
                    "GetDataDeletionRequestsPagedQuery_*"
                }
            },

            // ==================== NOTIFICATION ====================
            { typeof(CreateNotificationCommand), new[] { "GetUserNotificationsQuery_*" } },
            { typeof(MarkNotificationAsReadCommand), new[] { "GetUserNotificationsQuery_*" } },
            { typeof(MarkAllNotificationsAsReadCommand), new[] { "GetUserNotificationsQuery_*" } },
            { typeof(DeleteNotificationCommand), new[] { "GetUserNotificationsQuery_*" } },

            // ==================== DEVICE / LOGIN SESSION ====================
            { typeof(RegisterDeviceCommand), new[] { "GetUserDevicesQuery_*", "GetDeviceByIdQuery_*" } },
            { typeof(UpdateDeviceCommand), new[] { "GetUserDevicesQuery_*", "GetDeviceByIdQuery_*{Id}*" } },
            { typeof(DeleteDeviceCommand), new[] { "GetUserDevicesQuery_*", "GetDeviceByIdQuery_*{Id}*" } },
            { typeof(RevokeLoginSessionCommand), new[] { "GetUserActiveSessionsQuery_*" } },
            { typeof(RevokeAllUserSessionsCommand), new[] { "GetUserActiveSessionsQuery_*" } },

            // ==================== PAYMENT METHOD ====================
            { typeof(AddPaymentMethodCommand), new[] { "GetUserPaymentMethodsQuery_*", "GetPaymentMethodByIdQuery_*" } },
            { typeof(UpdatePaymentMethodCommand), new[] { "GetUserPaymentMethodsQuery_*", "GetPaymentMethodByIdQuery_*{Id}*" } },
            { typeof(DeletePaymentMethodCommand), new[] { "GetUserPaymentMethodsQuery_*", "GetPaymentMethodByIdQuery_*{Id}*" } },
            { typeof(SetDefaultPaymentMethodCommand), new[] { "GetUserPaymentMethodsQuery_*", "GetPaymentMethodByIdQuery_*{Id}*" } },

            // ==================== SUBSCRIPTION ====================
            { typeof(CreateSubscriptionCommand), new[]
                {
                    "GetCurrentSubscriptionQuery_*",
                    "GetSubscriptionHistoryQuery_*",
                    "GetAdminSubscriptionsPagedQuery_*"
                }
            },
            { typeof(UpdateSubscriptionCommand), new[]
                {
                    "GetCurrentSubscriptionQuery_*",
                    "GetSubscriptionHistoryQuery_*",
                    "GetAdminSubscriptionsPagedQuery_*"
                }
            },
            { typeof(CancelSubscriptionCommand), new[]
                {
                    "GetCurrentSubscriptionQuery_*",
                    "GetSubscriptionHistoryQuery_*",
                    "GetAdminSubscriptionsPagedQuery_*"
                }
            },

            // ==================== INVOICE ====================
            // Invoices are never modified after creation.
            // No invalidation needed for read-only invoice data.

            // ==================== PERMISSION & ROLE ====================
            { typeof(CreatePermissionCommand), new[] { "GetAllPermissionsQuery_*", "GetPermissionsByGroupQuery_*", "GetPermissionByIdQuery_*" } },
            { typeof(UpdatePermissionCommand), new[]
                {
                    "GetPermissionByIdQuery_*{Id}*",
                    "GetAllPermissionsQuery_*",
                    "GetPermissionsByGroupQuery_*",
                    "GetRoleByIdQuery_*"          // roles contain permissions
                }
            },
            { typeof(DeletePermissionCommand), new[]
                {
                    "GetPermissionByIdQuery_*{Id}*",
                    "GetAllPermissionsQuery_*",
                    "GetPermissionsByGroupQuery_*",
                    "GetRoleByIdQuery_*"
                }
            },
            { typeof(CreateRoleCommand), new[] { "GetAllRolesQuery_*", "GetRoleByIdQuery_*", "GetAdminRolesPagedQuery_*" } },
            { typeof(UpdateRoleCommand), new[] { "GetRoleByIdQuery_*{Id}*", "GetAllRolesQuery_*", "GetAdminRolesPagedQuery_*" } },
            { typeof(DeleteRoleCommand), new[] { "GetRoleByIdQuery_*{Id}*", "GetAllRolesQuery_*", "GetAdminRolesPagedQuery_*" } },

            // ==================== WATCH PARTY ====================
            { typeof(CreateWatchPartyCommand), new[]
                {
                    "GetActiveWatchPartiesPagedQuery_*",
                    "GetWatchPartyByIdQuery_*",
                    "GetWatchPartyByCodeQuery_*"
                }
            },
            { typeof(UpdateWatchPartyCommand), new[]
                {
                    "GetWatchPartyByIdQuery_*{Id}*",
                    "GetActiveWatchPartiesPagedQuery_*",
                    "GetWatchPartyByCodeQuery_*"
                }
            },
            { typeof(EndWatchPartyCommand), new[]
                {
                    "GetWatchPartyByIdQuery_*{Id}*",
                    "GetActiveWatchPartiesPagedQuery_*",
                    "GetWatchPartyByCodeQuery_*"
                }
            },
            { typeof(JoinWatchPartyCommand), new[] { "GetParticipantsByPartyQuery_*" } },
            { typeof(LeaveWatchPartyCommand), new[] { "GetParticipantsByPartyQuery_*" } },

            // ==================== SEARCH LOG ====================
            // Search logs are write‑only, no invalidation needed.

            // ==================== AUDIT LOG ====================
            // Audit logs are write‑only, no invalidation needed.

            // ==================== ERROR LOG ====================
            // Error logs are write‑only, no invalidation needed.

            // ==================== PERSONALIZED ROW ====================
            { typeof(UpsertPersonalizedRowCommand), new[] { "GetPersonalizedRowsByProfileQuery_*" } },
            { typeof(DeletePersonalizedRowCommand), new[] { "GetPersonalizedRowsByProfileQuery_*" } },
            { typeof(RegenerateRecommendationsCommand), new[] { "GetPersonalizedRowsByProfileQuery_*" } },

            // ==================== ITEM VECTOR ====================
            { typeof(UpsertItemVectorCommand), new[] { "GetItemVectorByShowIdQuery_*", "GetShowByIdQuery_*{ShowId}*" } },

            // ==================== USER VECTOR ====================
            { typeof(UpsertUserVectorCommand), new[] { "GetUserVectorByProfileIdQuery_*" } },

            // ==================== PUSH TOKEN ====================
            { typeof(RegisterPushTokenCommand), new[] { "GetUserPushTokensQuery_*" } },
            { typeof(DeletePushTokenCommand), new[] { "GetUserPushTokensQuery_*" } },

            // ==================== EMAIL PREFERENCE ====================
            { typeof(UpdateEmailPreferenceCommand), new[] { "GetEmailPreferenceQuery_*" } },

            // ==================== FRIENDSHIP ====================
            { typeof(SendFriendRequestCommand), new[]
                {
                    "GetFriendsQuery_*",
                    "GetFriendshipSummaryQuery_*",
                    "GetPendingRequestsQuery_*",
                    "SearchFriendsQuery_*"
                }
            },
            { typeof(RespondToFriendRequestCommand), new[]
                {
                    "GetFriendsQuery_*",
                    "GetFriendshipSummaryQuery_*",
                    "GetPendingRequestsQuery_*",
                    "SearchFriendsQuery_*"
                }
            },
            { typeof(Commands.Friendship.BlockUser.BlockUserCommand), new[]
                {
                    "GetFriendsQuery_*",
                    "GetFriendshipSummaryQuery_*",
                    "SearchFriendsQuery_*"
                }
            },
            { typeof(UnfriendCommand), new[]
                {
                    "GetFriendsQuery_*",
                    "GetFriendshipSummaryQuery_*",
                    "SearchFriendsQuery_*"
                }
            },

            // ==================== OFFLINE DOWNLOAD ====================
            { typeof(CreateOfflineDownloadCommand), new[] { "GetProfileDownloadsQuery_*" } },
            { typeof(DeleteOfflineDownloadCommand), new[] { "GetProfileDownloadsQuery_*" } },

            // ==================== PLAYBACK EVENT ====================
            // Playback events are write‑only, no invalidation needed (admin queries have very short TTL anyway).
        };


    }
}
