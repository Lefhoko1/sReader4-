# Infrastructure · Notifications

Reserved for device-level notification delivery (Phase 6):

- Android local notifications (`com.unity.mobile.notifications`) for
  assignment reminders while the app is closed.
- Push delivery later if a push provider is added.

In-app notification storage/listing already lives in
`Domains/Notifications` (`NotificationService` + `INotificationRepository`)
— this folder is only for the device-facing dispatcher implementation,
behind an interface defined in the Notifications domain when Phase 6 lands.
