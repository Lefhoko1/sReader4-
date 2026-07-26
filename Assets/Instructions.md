This is exactly the type of document I would hand to a senior engineer or AI coding agent before a single feature is implemented.

# Educational Multiplayer Platform - Architecture & Development Blueprint

## Project Vision

This project is not a simple Unity application.

It is a long-term educational multiplayer platform combining:

* Learning Management System (LMS)
* Educational Game
* Social Network
* Assignment Management Platform
* Multiplayer Experience
* Offline-First Application
* Content Distribution System

Expected future systems include:

* Academies
* Classes
* Modules
* Lessons
* Assignments
* Assignment Scheduling
* Assignment Submission
* Guardians
* Tutors
* Students
* Friends System
* Privacy Controls
* Notifications
* Multiplayer Networking
* Asset Downloads
* Local Content Storage
* Profile Images
* File Versioning
* User Locations
* Scheduled Activities
* Real-Time Synchronization

The architecture must be designed from day one to support growth over multiple years without requiring major refactoring.

---

# Architectural Principles

## Mandatory Principles

The codebase MUST follow:

* SOLID Principles
* Clean Architecture
* MVVM Pattern
* Repository Pattern
* Dependency Injection
* Domain Driven Design (DDD)
* Async Programming
* Event Driven Communication

The project must avoid:

* God Classes
* Service Locators
* Direct Database Access from UI
* Direct Supabase Calls from UI
* Business Logic inside MonoBehaviours
* Massive Managers

---

# High-Level Architecture

```text
Presentation Layer
    UI Toolkit (UXML)

        ↓

View Layer

        ↓

ViewModels

        ↓

Application Services

        ↓

Domain Layer

        ↓

Repositories

        ↓

Infrastructure Layer

        ↓

Supabase
SQLite
File Storage
Networking
Resend API
```

---

# Project Folder Structure

```text
Assets
│
├── Scripts
│
├── Core
│   ├── DependencyInjection
│   ├── Events
│   ├── Logging
│   ├── Configuration
│   ├── Authentication
│   └── Common
│
├── Domains
│
│   ├── Identity
│   ├── Education
│   ├── Assignments
│   ├── Social
│   ├── Multiplayer
│   ├── Scheduling
│   ├── Notifications
│   ├── Guardians
│   ├── Locations
│   ├── Files
│   └── Administration
│
├── Infrastructure
│
│   ├── Supabase
│   ├── SQLite
│   ├── Networking
│   ├── Storage
│   ├── Resend
│   └── Notifications
│
├── UI
│
│   ├── Views
│   ├── ViewModels
│   ├── Navigation
│   └── Bindings
│
└── Tests
```

---

# Domain Definitions

## Identity Domain

Responsible for:

* Authentication
* User Accounts
* Roles
* Profiles
* Privacy Settings

### User

Fields:

```text
Id
Email
Username
DisplayName
Role
Status
CreatedAt
UpdatedAt
```

### UserProfile

Fields:

```text
Id
UserId
Bio
Country
Timezone
ProfileImageId
```

### PrivacySettings

Fields:

```text
Id
UserId

ShowProfile
ShowSchedule
ShowLocation
ShowAssignments
ShowOnlineStatus
ShowFriends
```

---

# Guardian Domain

### GuardianStudent

Fields:

```text
GuardianId
StudentId
AssignedAt
```

Capabilities:

* Assign Student
* Remove Student
* List Students

---

# Education Domain

## Academy

Fields:

```text
Id
Name
Description
CreatedAt
```

---

## AcademyClass

Fields:

```text
Id
AcademyId
Name
Description
MaxStudents
```

---

## Module

Fields:

```text
Id
ClassId
Title
OrderIndex
```

---

## Lesson

Fields:

```text
Id
ModuleId
Title
Description
ContentPath
```

---

## Enrollment

Fields:

```text
StudentId
ClassId
EnrollmentDate
```

---

# Assignment Domain

## Assignment

Fields:

```text
Id
ClassId
ModuleId

Title
Instructions

DueDate

MaxScore

Version

CreatedAt
UpdatedAt
```

---

## AssignmentFile

Fields:

```text
Id
AssignmentId
Version
StoragePath
Checksum
```

---

## AssignmentAttempt

Fields:

```text
Id
AssignmentId
StudentId

StartedAt
CompletedAt

Status
```

Status:

```text
NotStarted
InProgress
Completed
Submitted
Graded
```

---

## AssignmentSubmission

Fields:

```text
Id
AttemptId
SubmittedAt

FilePath

Grade
Feedback
```

---

# Scheduling Domain

## Schedule

Fields:

```text
Id
UserId

Title
Description

StartTime
EndTime

ScheduleType
```

Types:

```text
Assignment
Class
Lesson
Meeting
Reminder
Event
```

---

## SchedulePermission

Fields:

```text
ScheduleId
UserId
CanView
```

---

# Social Domain

## Friendship

Fields:

```text
Id

RequesterId
RecipientId

Status
CreatedAt
```

Status:

```text
Pending
Accepted
Declined
Blocked
```

---

## FriendSettings

Fields:

```text
UserId

CanSeeProfile
CanSeeSchedule
CanSeeAssignments
CanSeeLocation
CanSeeOnlineStatus
```

---

# Notifications Domain

## Notification

Fields:

```text
Id
UserId

Title
Message

NotificationType

IsRead

CreatedAt
```

Types:

```text
AssignmentReminder
FriendRequest
AssignmentSubmitted
ClassInvite
MultiplayerInvite
```

---

# Multiplayer Domain

IMPORTANT:

Live multiplayer state must NEVER be stored in Supabase.

Supabase stores persistence only.

---

## PlayerProfile

Fields:

```text
UserId

Level
Experience

AvatarVersion
```

---

## PlayerState

Fields:

```text
UserId

LastScene
LastPosition

LastLogin
```

---

## MultiplayerSession

Fields:

```text
Id

HostUserId

SessionName

MaxPlayers

CreatedAt
```

---

# Location Domain

## UserLocation

Fields:

```text
Id
UserId

Country
Province
City
Address

Latitude
Longitude
```

---

# Files Domain

This is critical.

All downloadable content must support versioning.

---

## AssetVersion

Fields:

```text
Id

AssetType

AssetId

Version

Checksum

CreatedAt
```

Asset Types:

```text
ProfileImage
Assignment
Lesson
Avatar
```

---

# Local SQLite Design

SQLite is NOT a mirror of Supabase.

SQLite is only for:

* Offline access
* Caching
* Downloads
* Pending sync operations

Tables:

```text
CachedAssignments
CachedLessons
CachedProfiles
CachedFriends
CachedImages
PendingUploads
PendingDownloads
PendingSyncOperations
```

---

# Synchronization System

Create a dedicated Sync Domain.

Never sync from UI.

---

## SyncService

Responsibilities:

* Download updates
* Upload changes
* Resolve conflicts
* Handle retries
* Detect version changes

---

## Sync Queue

Fields:

```text
Id

OperationType

EntityType

EntityId

CreatedAt

Status
```

---

# MVVM Requirements

Each screen must have:

```text
View
ViewModel
Service
Repository
```

Example:

```text
LoginView

LoginViewModel

AuthenticationService

AuthenticationRepository
```

Never skip layers.

---

# Core Services

## AuthenticationService

Responsibilities:

* Register
* Login
* Logout
* Session Restore

---

## UserService

Responsibilities:

* Load Users
* Update Users
* Manage Profiles

---

## AssignmentService

Responsibilities:

* Assignment Management
* Assignment Scheduling
* Assignment Submission

---

## FriendshipService

Responsibilities:

* Friend Requests
* Accept Requests
* Remove Friends

---

## ScheduleService

Responsibilities:

* Schedule Management
* Visibility Rules

---

## NotificationService

Responsibilities:

* Reminder Generation
* Notification Delivery

---

## SyncService

Responsibilities:

* Cloud Sync
* Offline Sync
* Conflict Resolution

---

# Networking Architecture

Recommended:

Unity Netcode for GameObjects

OR

Mirror Networking

Abstract networking behind interfaces.

Example:

```text
INetworkService

NetcodeNetworkService
MirrorNetworkService
```

Game systems must never depend directly on networking implementation.

---

# Development Phases

## Phase 1 - Foundation

Implement:

* Dependency Injection
* MVVM Framework
* Logging
* Configuration
* Event System
* Supabase Integration
* SQLite Integration

NO gameplay yet.

---

## Phase 2 - Identity

Implement:

* Registration
* Login
* Roles
* Profiles
* Privacy Settings

---

## Phase 3 - Education

Implement:

* Academies
* Classes
* Modules
* Lessons
* Enrollment

---

## Phase 4 - Assignments

Implement:

* Assignment Downloads
* Attempts
* Submissions
* Grading
* Scheduling

---

## Phase 5 - Social

Implement:

* Friend Requests
* Friends List
* Visibility Rules

---

## Phase 6 - Notifications

Implement:

* Assignment Reminders
* Friend Notifications
* Class Notifications

---

## Phase 7 - Sync System

Implement:

* SQLite Cache
* Sync Queue
* Conflict Resolution
* Asset Versioning

---

## Phase 8 - Multiplayer

Implement:

* Player Profiles
* Multiplayer Sessions
* Player Spawning
* Session Persistence

---

## Phase 9 - Advanced Systems

Implement:

* Clubs
* Guilds
* Competitions
* Achievements
* Marketplace
* AI Tutors

---

# Non-Negotiable Rules

1. UI never talks directly to Supabase.
2. UI never talks directly to SQLite.
3. UI never talks directly to Networking.
4. Business logic never lives in MonoBehaviours.
5. Every feature belongs to a domain.
6. Every domain owns its own models.
7. Every external system is accessed through interfaces.
8. Every cloud operation must be asynchronous.
9. Every downloadable asset must support versioning.
10. Multiplayer state and educational state must remain separate concerns.

The implementation should prioritize long-term maintainability over short-term development speed. Every architectural decision should assume the platform will continue expanding for several years.

This document is strong enough to use as the "constitution" for the project. I'd actually recommend making Claude follow it before writing any code and asking it to first scaffold the entire architecture (folders, interfaces, base models, DI container, MVVM framework, repositories, services, and sync infrastructure) before implementing a single feature. That alone will save months of refactoring later.
