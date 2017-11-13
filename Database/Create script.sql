
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogActivity](
	[LogActivityId] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](256) NULL,
	[LogActivityDefinitionId] [bigint] NULL,
 CONSTRAINT [PK_LogActivity] PRIMARY KEY CLUSTERED 
(
	[LogActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogActivityDefinition]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogActivityDefinition](
	[LogActivityDefinitionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](256) NULL,
	[MoreInfo] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogActivityDefinition] PRIMARY KEY CLUSTERED 
(
	[LogActivityDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogActivityDefinitionDetail]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogActivityDefinitionDetail](
	[LogActivityDefinitionDetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogActivityDefinitionDetailTypeId] [bigint] NOT NULL,
	[LogActivityId] [bigint] NULL,
	[Language] [nvarchar](10) NULL,
	[Label] [nvarchar](1024) NULL,
 CONSTRAINT [PK_LogActivityDefinitionDetail] PRIMARY KEY CLUSTERED 
(
	[LogActivityDefinitionDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogActivityDefinitionDetailType]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogActivityDefinitionDetailType](
	[LogActivityDefinitionDetailTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogActivityDefinitionDetailType] PRIMARY KEY CLUSTERED 
(
	[LogActivityDefinitionDetailTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogAgent]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogAgent](
	[LogAgentId] [bigint] NOT NULL,
	[StudentId] [bigint] NULL,
	[Name] [nvarchar](256) NULL,
	[Mbox] [nvarchar](256) NULL,
	[Mbox_sha1sum] [nvarchar](256) NULL,
	[OpenId] [nvarchar](256) NULL,
	[LogAgentAccountId] [bigint] NULL,
	[Password]	[nvarchar](MAX)	null,
	[PasswordSalt]	[nvarchar](MAX)	null,
 CONSTRAINT [PK_LogAgent] PRIMARY KEY CLUSTERED 
(
	[LogAgentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogAgentAccount]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogAgentAccount](
	[LogAgentAccountId] [bigint] IDENTITY(1,1) NOT NULL,
	[Homepage] [nvarchar](256) NULL,
	[Name] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogAgentAccount] PRIMARY KEY CLUSTERED 
(
	[LogAgentAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogContext]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogContext](
	[LogContextId] [bigint] IDENTITY(1,1) NOT NULL,
	[Registration] [uniqueidentifier] NULL,
	[InstructorLogAgentId] [bigint] NULL,
	[TeamLogAgentId] [bigint] NULL,
	[Revision] [nvarchar](50) NULL,
	[Platform] [nvarchar](50) NULL,
	[Language] [nvarchar](10) NULL,
	[RefLogStatementId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_LogContext] PRIMARY KEY CLUSTERED 
(
	[LogContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogContextActivity]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogContextActivity](
	[LogContextId] [bigint] NOT NULL,
	[LogContextActivityTypeId] [bigint] NOT NULL,
	[LogActivityId] [bigint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogContextActivityType]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogContextActivityType](
	[LogContextActivityTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogContextActivityType] PRIMARY KEY CLUSTERED 
(
	[LogContextActivityTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogExtension]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogExtension](
	[LogContextId] [bigint] NULL,
	[LogResultId] [bigint] NULL,
	[LogActivityDefinitionId] [bigint] NULL,
	[LogActivityId] [bigint] NULL,
	[Uri] [nvarchar](256) NULL,
	[Token] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogResult]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogResult](
	[LogResultId] [bigint] IDENTITY(1,1) NOT NULL,
	[IsCompleted] [bit] NULL,
	[IsSuccess] [bit] NULL,
	[Response] [nvarchar](max) NULL,
	[DurationTicks] [bigint] NULL,
	[LogScoreId] [bigint] NULL,
 CONSTRAINT [PK_LogResult] PRIMARY KEY CLUSTERED 
(
	[LogResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogScore]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogScore](
	[LogScoreId] [bigint] IDENTITY(1,1) NOT NULL,
	[Scaled] [float] NULL,
	[Raw] [float] NULL,
	[Min] [float] NULL,
	[Max] [float] NULL,
 CONSTRAINT [PK_LogScore] PRIMARY KEY CLUSTERED 
(
	[LogScoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogStatement]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogStatement](
	[LogStatementId] [uniqueidentifier] NOT NULL,
	[LogAgentId] [bigint] NULL,
	[LogVerbId] [bigint] NULL,
	[TargetLogAgentId] [bigint] NULL,
	[TargetLogActivityId] [bigint] NULL,
	[TargetLogStatementId] [uniqueidentifier] NULL,
	[LogResultId] [bigint] NULL,
	[LogContextId] [bigint] NULL,
	[Timestamp] [datetime2](7) NULL,
	[StoredTimestamp] [datetime2](7) NULL,
	[AuthorityLogAgentId] [bigint] NULL,
 CONSTRAINT [PK_LogStatement] PRIMARY KEY CLUSTERED 
(
	[LogStatementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogStatementLink]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogStatementLink](
	[LogStatementId] [uniqueidentifier] NULL,
	[TableName] [nvarchar](50) NULL,
	[TableId] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogVerb]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogVerb](
	[LogVerbId] [bigint] IDENTITY(100,1) NOT NULL,
	[Uri] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogVerb] PRIMARY KEY CLUSTERED 
(
	[LogVerbId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogVerbLabel]    Script Date: 20/05/2016 13:39:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogVerbLabel](
	[LogVerbId] [bigint] NULL,
	[Language] [nvarchar](10) NULL,
	[Label] [nvarchar](256) NULL
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[LogActivity]  WITH CHECK ADD  CONSTRAINT [FK_LogActivity_LogActivityDefinition] FOREIGN KEY([LogActivityDefinitionId])
REFERENCES [dbo].[LogActivityDefinition] ([LogActivityDefinitionId])
GO
ALTER TABLE [dbo].[LogActivity] CHECK CONSTRAINT [FK_LogActivity_LogActivityDefinition]
GO
ALTER TABLE [dbo].[LogActivityDefinitionDetail]  WITH CHECK ADD  CONSTRAINT [FK_LogActivityDefinitionDetail_LogActivity] FOREIGN KEY([LogActivityId])
REFERENCES [dbo].[LogActivity] ([LogActivityId])
GO
ALTER TABLE [dbo].[LogActivityDefinitionDetail] CHECK CONSTRAINT [FK_LogActivityDefinitionDetail_LogActivity]
GO
ALTER TABLE [dbo].[LogActivityDefinitionDetail]  WITH CHECK ADD  CONSTRAINT [FK_LogActivityDefinitionDetail_LogActivityDefinitionDetailType] FOREIGN KEY([LogActivityDefinitionDetailTypeId])
REFERENCES [dbo].[LogActivityDefinitionDetailType] ([LogActivityDefinitionDetailTypeId])
GO
ALTER TABLE [dbo].[LogActivityDefinitionDetail] CHECK CONSTRAINT [FK_LogActivityDefinitionDetail_LogActivityDefinitionDetailType]
GO
ALTER TABLE [dbo].[LogAgent]  WITH CHECK ADD  CONSTRAINT [FK_LogAgent_LogAgentAccount] FOREIGN KEY([LogAgentAccountId])
REFERENCES [dbo].[LogAgentAccount] ([LogAgentAccountId])
GO
ALTER TABLE [dbo].[LogAgent] CHECK CONSTRAINT [FK_LogAgent_LogAgentAccount]
GO
ALTER TABLE [dbo].[LogContext]  WITH CHECK ADD  CONSTRAINT [FK_LogContext_LogAgent] FOREIGN KEY([InstructorLogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogContext] CHECK CONSTRAINT [FK_LogContext_LogAgent]
GO
ALTER TABLE [dbo].[LogContext]  WITH CHECK ADD  CONSTRAINT [FK_LogContext_LogAgent1] FOREIGN KEY([TeamLogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogContext] CHECK CONSTRAINT [FK_LogContext_LogAgent1]
GO
ALTER TABLE [dbo].[LogContext]  WITH CHECK ADD  CONSTRAINT [FK_LogContext_LogStatement] FOREIGN KEY([RefLogStatementId])
REFERENCES [dbo].[LogStatement] ([LogStatementId])
GO
ALTER TABLE [dbo].[LogContext] CHECK CONSTRAINT [FK_LogContext_LogStatement]
GO
ALTER TABLE [dbo].[LogContextActivity]  WITH CHECK ADD  CONSTRAINT [FK_LogContextActivity_LogActivity] FOREIGN KEY([LogActivityId])
REFERENCES [dbo].[LogActivity] ([LogActivityId])
GO
ALTER TABLE [dbo].[LogContextActivity] CHECK CONSTRAINT [FK_LogContextActivity_LogActivity]
GO
ALTER TABLE [dbo].[LogContextActivity]  WITH CHECK ADD  CONSTRAINT [FK_LogContextActivity_LogContext] FOREIGN KEY([LogContextId])
REFERENCES [dbo].[LogContext] ([LogContextId])
GO
ALTER TABLE [dbo].[LogContextActivity] CHECK CONSTRAINT [FK_LogContextActivity_LogContext]
GO
ALTER TABLE [dbo].[LogContextActivity]  WITH CHECK ADD  CONSTRAINT [FK_LogContextActivity_LogContextActivityType] FOREIGN KEY([LogContextActivityTypeId])
REFERENCES [dbo].[LogContextActivityType] ([LogContextActivityTypeId])
GO
ALTER TABLE [dbo].[LogContextActivity] CHECK CONSTRAINT [FK_LogContextActivity_LogContextActivityType]
GO
ALTER TABLE [dbo].[LogExtension]  WITH CHECK ADD  CONSTRAINT [FK_LogExtension_LogActivity] FOREIGN KEY([LogActivityId])
REFERENCES [dbo].[LogActivity] ([LogActivityId])
GO
ALTER TABLE [dbo].[LogExtension] CHECK CONSTRAINT [FK_LogExtension_LogActivity]
GO
ALTER TABLE [dbo].[LogExtension]  WITH CHECK ADD  CONSTRAINT [FK_LogExtension_LogActivityDefinition] FOREIGN KEY([LogActivityDefinitionId])
REFERENCES [dbo].[LogActivityDefinition] ([LogActivityDefinitionId])
GO
ALTER TABLE [dbo].[LogExtension] CHECK CONSTRAINT [FK_LogExtension_LogActivityDefinition]
GO
ALTER TABLE [dbo].[LogExtension]  WITH CHECK ADD  CONSTRAINT [FK_LogExtension_LogContext] FOREIGN KEY([LogContextId])
REFERENCES [dbo].[LogContext] ([LogContextId])
GO
ALTER TABLE [dbo].[LogExtension] CHECK CONSTRAINT [FK_LogExtension_LogContext]
GO
ALTER TABLE [dbo].[LogExtension]  WITH CHECK ADD  CONSTRAINT [FK_LogExtension_LogResult] FOREIGN KEY([LogResultId])
REFERENCES [dbo].[LogResult] ([LogResultId])
GO
ALTER TABLE [dbo].[LogExtension] CHECK CONSTRAINT [FK_LogExtension_LogResult]
GO
ALTER TABLE [dbo].[LogResult]  WITH CHECK ADD  CONSTRAINT [FK_LogResult_LogScore] FOREIGN KEY([LogScoreId])
REFERENCES [dbo].[LogScore] ([LogScoreId])
GO
ALTER TABLE [dbo].[LogResult] CHECK CONSTRAINT [FK_LogResult_LogScore]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogActivity] FOREIGN KEY([TargetLogActivityId])
REFERENCES [dbo].[LogActivity] ([LogActivityId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogActivity]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogAgent] FOREIGN KEY([AuthorityLogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogAgent]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogAgent1] FOREIGN KEY([LogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogAgent1]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogAgent2] FOREIGN KEY([TargetLogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogAgent2]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogContext] FOREIGN KEY([LogContextId])
REFERENCES [dbo].[LogContext] ([LogContextId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogContext]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogResult] FOREIGN KEY([LogResultId])
REFERENCES [dbo].[LogResult] ([LogResultId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogResult]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogStatement1] FOREIGN KEY([TargetLogStatementId])
REFERENCES [dbo].[LogStatement] ([LogStatementId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogStatement1]
GO
ALTER TABLE [dbo].[LogStatement]  WITH CHECK ADD  CONSTRAINT [FK_LogStatement_LogVerb] FOREIGN KEY([LogVerbId])
REFERENCES [dbo].[LogVerb] ([LogVerbId])
GO
ALTER TABLE [dbo].[LogStatement] CHECK CONSTRAINT [FK_LogStatement_LogVerb]
GO
ALTER TABLE [dbo].[LogVerbLabel]  WITH CHECK ADD  CONSTRAINT [FK_LogVerbLabel_LogVerb] FOREIGN KEY([LogVerbId])
REFERENCES [dbo].[LogVerb] ([LogVerbId])
GO
ALTER TABLE [dbo].[LogVerbLabel] CHECK CONSTRAINT [FK_LogVerbLabel_LogVerb]
GO

SET IDENTITY_INSERT LogVerb ON
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (1, N'https://w3id.org/xapi/adl/verbs/logged-in')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (2, N'https://w3id.org/xapi/adl/verbs/logged-out')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (3, N'http://activitystrea.ms/schema/1.0/access')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (4, N'http://activitystrea.ms/schema/1.0/complete')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (5, N'http://adlnet.gov/expapi/verbs/attempted')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (6, N'http://activitystrea.ms/schema/1.0/search')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (7, N'http://id.tincanapi.com/verb/viewed')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (8, N'http://www.project-vital.eu/xapi/verb/voice-recorded')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (9, N'http://activitystrea.ms/schema/1.0/play')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (10, N'http://id.tincanapi.com/verb/paused')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (11, N'http://activitystrea.ms/schema/1.0/watch')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (12, N'http://activitystrea.ms/schema/1.0/listen')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (13, N'http://adlnet.gov/expapi/activities/link')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (14, N'http://id.tincanapi.com/verb/previewed')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (15, N'http://www.project-vital.eu/xapi/verb/printed-to-pdf')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (16, N'http://id.tincanapi.com/verb/skipped')
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (17, N'http://adlnet.gov/expapi/verbs/interacted')
GO
SET IDENTITY_INSERT LogVerb OFF
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (1, N'en-uk', N'logged in')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (1, N'nl-be', N'ingelogd')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (2, N'en-uk', N'logged out')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (2, N'nl-be', N'uitgelogd')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (3, N'en-uk', N'accessed')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (3, N'nl-be', N'geopend')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (4, N'en-uk', N'completed')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (4, N'nl-be', N'voltooid')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (5, N'en-uk', N'attempted')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (5, N'nl-be', N'geprobeerd')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (6, N'en-uk', N'searched')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (6, N'nl-be', N'gezocht')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (7, N'en-uk', N'viewed')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (7, N'nl-be', N'bekeken')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (8, N'en-uk', N'recorded')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (8, N'nl-be', N'opgenomen')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (9, N'en-uk', N'played')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (9, N'nl-be', N'afgespeeld')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (10, N'en-uk', N'paused')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (10, N'nl-be', N'gepauzeerd')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (11, N'en-uk', N'watched')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (11, N'nl-be', N'volledig bekeken')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (12, N'en-uk', N'listened')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (12, N'nl-be', N'volledig geluisterd')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (13, N'en-uk', N'linked')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (13, N'nl-be', N'gelinkt')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (14, N'en-uk', N'previewed')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (14, N'nl-be', N'kort bekeken')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (15, N'en-uk', N'printed')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (15, N'nl-be', N'geprint')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (16, N'en-uk', N'skipped')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (16, N'nl-be', N'overgeslagen')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (17, N'en-uk', N'interacted')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (17, N'nl-be', N'geïnterageerd')
GO
SET IDENTITY_INSERT [dbo].[LogActivityDefinition] ON 

GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (1, N'http://activitystrea.ms/schema/1.0/application', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (2, N'http://adlnet.gov/expapi/activities/course', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (3, N'http://adlnet.gov/expapi/activities/module', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (4, N'http://adlnet.gov/expapi/activities/interaction', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (5, N'http://activitystrea.ms/schema/1.0/page', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (6, N'http://activitystrea.ms/schema/1.0/article', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (7, N'http://activitystrea.ms/schema/1.0/page', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (8, N'http://activitystrea.ms/schema/1.0/article', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (9, N'http://adlnet.gov/expapi/activities/media', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (10, N'http://adlnet.gov/expapi/activities/link', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (11, N'http://activitystrea.ms/schema/1.0/file', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (12, N'http://activitystrea.ms/schema/1.0/article', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (13, N'http://activitystrea.ms/schema/1.0/article', NULL)
GO
INSERT [dbo].[LogActivityDefinition] ([LogActivityDefinitionId], [Type], [MoreInfo]) VALUES (14, N'http://activitystrea.ms/schema/1.0/article', NULL)
GO
SET IDENTITY_INSERT [dbo].[LogActivityDefinition] OFF
GO
SET IDENTITY_INSERT [dbo].[LogActivityDefinitionDetailType] ON 


GO
ALTER TABLE [LogActivityDefinition] ADD Name NVARCHAR(64) NULL ;  
GO
Update LogActivityDefinition set Name = 'Application' where LogActivityDefinitionId = 1;
GO
Update LogActivityDefinition set Name = 'Course' where LogActivityDefinitionId = 2;
GO
Update LogActivityDefinition set Name = 'Module' where LogActivityDefinitionId = 3;
GO
Update LogActivityDefinition set Name = 'Interaction' where LogActivityDefinitionId = 4;
GO
Update LogActivityDefinition set Name = 'Page_Theory' where LogActivityDefinitionId = 5;
GO
Update LogActivityDefinition set Name = 'Article_Hint' where LogActivityDefinitionId = 6;
GO
Update LogActivityDefinition set Name = 'Page_Dictionary' where LogActivityDefinitionId = 7;
GO
Update LogActivityDefinition set Name = 'Article_Help' where LogActivityDefinitionId = 8;
GO
Update LogActivityDefinition set Name = 'Media' where LogActivityDefinitionId = 9;
GO
Update LogActivityDefinition set Name = 'Link_External' where LogActivityDefinitionId = 10;
GO
Update LogActivityDefinition set Name = 'File' where LogActivityDefinitionId = 11;
GO
Update LogActivityDefinition set Name = 'Article_Solution' where LogActivityDefinitionId = 12;
GO
Update LogActivityDefinition set Name = 'Article_Feedback' where LogActivityDefinitionId = 13;
GO
Update LogActivityDefinition set Name = 'Article_ExploreHotspot' where LogActivityDefinitionId = 14;





GO
INSERT [dbo].[LogActivityDefinitionDetailType] ([LogActivityDefinitionDetailTypeId], [Label]) VALUES (1, N'name')
GO
INSERT [dbo].[LogActivityDefinitionDetailType] ([LogActivityDefinitionDetailTypeId], [Label]) VALUES (2, N'description')
GO
SET IDENTITY_INSERT [dbo].[LogActivityDefinitionDetailType] OFF
GO
SET IDENTITY_INSERT [dbo].[LogContextActivityType] ON 

GO
INSERT [dbo].[LogContextActivityType] ([LogContextActivityTypeId], [Type]) VALUES (1, N'parent')
GO
INSERT [dbo].[LogContextActivityType] ([LogContextActivityTypeId], [Type]) VALUES (2, N'grouping')
GO
INSERT [dbo].[LogContextActivityType] ([LogContextActivityTypeId], [Type]) VALUES (3, N'category')
GO
INSERT [dbo].[LogContextActivityType] ([LogContextActivityTypeId], [Type]) VALUES (4, N'other')
GO
SET IDENTITY_INSERT [dbo].[LogContextActivityType] OFF
GO

SET IDENTITY_INSERT LogVerb ON
GO
INSERT [dbo].[LogVerb] ([LogVerbId], [Uri]) VALUES (18, N'https://w3id.org/xapi/adl/verbs/abandoned')
GO
SET IDENTITY_INSERT LogVerb OFF
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (18, N'en-uk', N'session timed out')
GO
INSERT [dbo].[LogVerbLabel] ([LogVerbId], [Language], [Label]) VALUES (18, N'nl-be', N'sessie verlopen')
GO
update LogVerbLabel set Language = 'en-gb' where Language='en-uk'


GO
ALTER TABLE LogAgent ADD AnonymousKey uniqueidentifier


GO

/****** Object:  Index [IX_LogActivity]    Script Date: 11/8/2016 2:45:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogActivity] ON [dbo].[LogActivity]
(
	[LogActivityDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogActivityDefinitionDetail]    Script Date: 11/8/2016 2:44:46 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogActivityDefinitionDetail] ON [dbo].[LogActivityDefinitionDetail]
(
	[LogActivityDefinitionDetailTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogActivityDefinitionDetail2]    Script Date: 11/8/2016 2:44:59 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogActivityDefinitionDetail2] ON [dbo].[LogActivityDefinitionDetail]
(
	[LogActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogAgent]    Script Date: 11/8/2016 2:41:21 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogAgent] ON [dbo].[LogAgent]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_LogAgent2]    Script Date: 11/8/2016 2:43:03 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogAgent2] ON [dbo].[LogAgent]
(
	[LogAgentAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogContext]    Script Date: 11/8/2016 2:47:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogContext] ON [dbo].[LogContext]
(
	[InstructorLogAgentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogContext2]    Script Date: 11/8/2016 2:47:21 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogContext2] ON [dbo].[LogContext]
(
	[TeamLogAgentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogContext3]    Script Date: 11/8/2016 2:47:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogContext3] ON [dbo].[LogContext]
(
	[RefLogStatementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogContext4]    Script Date: 11/8/2016 2:48:24 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogContext4] ON [dbo].[LogContext]
(
	[Registration] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO




CREATE NONCLUSTERED INDEX [IX_LogContextActivity] ON [dbo].[LogContextActivity]
(
	[LogContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO


CREATE NONCLUSTERED INDEX [IX_LogContextActivity2] ON [dbo].[LogContextActivity]
(
	[LogContextActivityTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO



CREATE NONCLUSTERED INDEX [IX_LogContextActivity3] ON [dbo].[LogContextActivity]
(
	[LogActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO




/****** Object:  Index [IX_LogExtension]    Script Date: 11/8/2016 2:55:08 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogExtension] ON [dbo].[LogExtension]
(
	[LogContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogExtension_1]    Script Date: 11/8/2016 2:55:16 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogExtension_1] ON [dbo].[LogExtension]
(
	[LogActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogExtension_2]    Script Date: 11/8/2016 2:55:23 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogExtension_2] ON [dbo].[LogExtension]
(
	[LogActivityDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogExtension_3]    Script Date: 11/8/2016 2:55:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogExtension_3] ON [dbo].[LogExtension]
(
	[LogResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogResult]    Script Date: 11/8/2016 2:56:54 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogResult] ON [dbo].[LogResult]
(
	[LogScoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogStatement]    Script Date: 11/8/2016 2:59:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogStatement] ON [dbo].[LogStatement]
(
	[StoredTimestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



/****** Object:  Index [IX_LogStatement_1]    Script Date: 11/8/2016 2:59:09 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogStatement_1] ON [dbo].[LogStatement]
(
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogStatementLink]    Script Date: 11/8/2016 3:00:40 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogStatementLink] ON [dbo].[LogStatementLink]
(
	[LogStatementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


/****** Object:  Index [IX_LogVerbLabel]    Script Date: 11/8/2016 3:01:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogVerbLabel] ON [dbo].[LogVerbLabel]
(
	[LogVerbId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO




/****** Object:  Table [dbo].[LogAgentMetadata]    Script Date: 5/04/2017 11:42:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogAgentMetadata](
	[LogAgentId] [bigint] NOT NULL,
	[LogMetadataId] [bigint] NOT NULL
) ON [PRIMARY]




GO
/****** Object:  Table [dbo].[LogExtensionMetadata]    Script Date: 5/04/2017 11:42:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogExtensionMetadata](
	[LogExtensionUri] [nvarchar](256) NOT NULL,
	[LogExtensionToken] [nvarchar](max) NOT NULL,
	[LogMetadataId] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogMetadata]    Script Date: 5/04/2017 11:42:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogMetadata](
	[LogMetadataId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogMetadataTypeId] [bigint] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_LogMetadata] PRIMARY KEY CLUSTERED 
(
	[LogMetadataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogMetadataType]    Script Date: 5/04/2017 11:42:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogMetadataType](
	[LogMetadataTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Descr] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_LogMetadataType] PRIMARY KEY CLUSTERED 
(
	[LogMetadataTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[LogAgentMetadata]  WITH CHECK ADD  CONSTRAINT [FK_LogAgentMetadata_LogAgent] FOREIGN KEY([LogAgentId])
REFERENCES [dbo].[LogAgent] ([LogAgentId])
GO
ALTER TABLE [dbo].[LogAgentMetadata] CHECK CONSTRAINT [FK_LogAgentMetadata_LogAgent]
GO
ALTER TABLE [dbo].[LogAgentMetadata]  WITH CHECK ADD  CONSTRAINT [FK_LogAgentMetadata_LogMetadata] FOREIGN KEY([LogMetadataId])
REFERENCES [dbo].[LogMetadata] ([LogMetadataId])
GO
ALTER TABLE [dbo].[LogAgentMetadata] CHECK CONSTRAINT [FK_LogAgentMetadata_LogMetadata]
GO
ALTER TABLE [dbo].[LogExtensionMetadata]  WITH CHECK ADD  CONSTRAINT [FK_LogExtensionMetadata_LogMetadata] FOREIGN KEY([LogMetadataId])
REFERENCES [dbo].[LogMetadata] ([LogMetadataId])
GO
ALTER TABLE [dbo].[LogExtensionMetadata] CHECK CONSTRAINT [FK_LogExtensionMetadata_LogMetadata]
GO
ALTER TABLE [dbo].[LogMetadata]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadata_LogMetadataType] FOREIGN KEY([LogMetadataTypeId])
REFERENCES [dbo].[LogMetadataType] ([LogMetadataTypeId])
GO
ALTER TABLE [dbo].[LogMetadata] CHECK CONSTRAINT [FK_LogMetadata_LogMetadataType]
GO



CREATE TABLE [dbo].[LogMetadataCourse](
	[LogMetadataCourseId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogMetadataCourse] PRIMARY KEY CLUSTERED 
(
	[LogMetadataCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LogMetadataCourseInstance](
	[LogMetadataCourseInstanceId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogMetadataCourseId] [bigint] NOT NULL,
	[AcademicYear] [nvarchar](32) NOT NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[UntilDate] [datetime2](7) NOT NULL,
	[ImportId] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_LogMetadataCourseInstance] PRIMARY KEY CLUSTERED 
(
	[LogMetadataCourseInstanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This string is used to match incoming metadata to an Instance of a Course, without knowing the underlying ID.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LogMetadataCourseInstance', @level2type=N'COLUMN',@level2name=N'ImportId'
GO


CREATE TABLE [dbo].[LogMetadataCourseInstanceTimeBlock](
	[LogMetadataCourseInstanceId] [bigint] NOT NULL,
	[TimeBlock] [int] NOT NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[UntilDate] [datetime2](7) NOT NULL
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[LogMetadataCourseProgramme](
	[LogMetadataCourseProgrammeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NULL,
 CONSTRAINT [PK_LogMetadataCourseProgramme] PRIMARY KEY CLUSTERED 
(
	[LogMetadataCourseProgrammeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



CREATE TABLE [dbo].[LogMetadataTeacher](
	[LogMetadataTeacherId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_LogMetadataTeacher] PRIMARY KEY CLUSTERED 
(
	[LogMetadataTeacherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



GO

CREATE TABLE [dbo].[LogMetadataCourseInstanceClass](
	[LogMetadataCourseInstanceClassId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogMetadataCourseInstanceId] [bigint] NOT NULL,
	[LogMetadataCourseProgrammeId] [bigint] NOT NULL,
	[LogMetadataCourseInstanceClassTypeId] [bigint] NOT NULL,
	[Group] [nvarchar](50) NULL,
	[LogMetadataTeacherId] [bigint] NULL,
	[FromDate] [datetime2](7) NOT NULL,
	[UntilDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_LogMetadataCourseInstanceClass] PRIMARY KEY CLUSTERED 
(
	[LogMetadataCourseInstanceClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LogMetadataCourseInstanceClassType](
	[LogMetadataCourseInstanceClassTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_LogMetadataCourseInstanceClassType] PRIMARY KEY CLUSTERED 
(
	[LogMetadataCourseInstanceClassTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LogMetadataActivityInCourse](
	[LogActivityUrl] [nvarchar](256) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[TimeBlock] [int] NULL,
	[LogMetadataCourseId] [bigint] NOT NULL,
	[Chapter] [nvarchar](256) NULL
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[LogMetadataAgentInCourseInstance](
	[LogAgentId] [bigint] NOT NULL,
	[LogMetadataCourseInstanceId] [bigint] NOT NULL,
	[LogMetadataCourseProgrammeId] [bigint] NOT NULL,
	[Group] [nvarchar](50) NULL,
	[Score] [decimal](18, 2) NULL,
	[ScoreMax] [decimal](18, 2) NULL,
	[CalculatedPerformanceGroup] [nvarchar](50) NULL
) ON [PRIMARY]

GO


/****** Object:  Index [IX_LogMetadataAgentInCourseInstance]    Script Date: 5/05/2017 14:16:04 ******/
CREATE NONCLUSTERED INDEX [IX_LogMetadataAgentInCourseInstance] ON [dbo].[LogMetadataAgentInCourseInstance]
(
	[LogAgentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO




ALTER TABLE [dbo].[LogMetadataCourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstance_LogMetadataCourse] FOREIGN KEY([LogMetadataCourseId])
REFERENCES [dbo].[LogMetadataCourse] ([LogMetadataCourseId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstance] CHECK CONSTRAINT [FK_LogMetadataCourseInstance_LogMetadataCourse]
GO


ALTER TABLE [dbo].[LogMetadataCourseInstanceTimeBlock]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstanceTimeBlock_LogMetadataCourseInstance] FOREIGN KEY([LogMetadataCourseInstanceId])
REFERENCES [dbo].[LogMetadataCourseInstance] ([LogMetadataCourseInstanceId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceTimeBlock] CHECK CONSTRAINT [FK_LogMetadataCourseInstanceTimeBlock_LogMetadataCourseInstance]
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseInstance] FOREIGN KEY([LogMetadataCourseInstanceId])
REFERENCES [dbo].[LogMetadataCourseInstance] ([LogMetadataCourseInstanceId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass] CHECK CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseInstance]
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseInstanceClassType] FOREIGN KEY([LogMetadataCourseInstanceClassTypeId])
REFERENCES [dbo].[LogMetadataCourseInstanceClassType] ([LogMetadataCourseInstanceClassTypeId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass] CHECK CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseInstanceClassType]
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseProgramme] FOREIGN KEY([LogMetadataCourseProgrammeId])
REFERENCES [dbo].[LogMetadataCourseProgramme] ([LogMetadataCourseProgrammeId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass] CHECK CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataCourseProgramme]
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataTeacher] FOREIGN KEY([LogMetadataTeacherId])
REFERENCES [dbo].[LogMetadataTeacher] ([LogMetadataTeacherId])
GO

ALTER TABLE [dbo].[LogMetadataCourseInstanceClass] CHECK CONSTRAINT [FK_LogMetadataCourseInstanceClass_LogMetadataTeacher]
GO

ALTER TABLE [dbo].[LogMetadataActivityInCourse]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataActivityInCourse_LogMetadataCourse] FOREIGN KEY([LogMetadataCourseId])
REFERENCES [dbo].[LogMetadataCourse] ([LogMetadataCourseId])
GO

ALTER TABLE [dbo].[LogMetadataActivityInCourse] CHECK CONSTRAINT [FK_LogMetadataActivityInCourse_LogMetadataCourse]
GO


ALTER TABLE [dbo].[LogMetadataAgentInCourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataAgentInCourseInstance_LogMetadataCourseInstance] FOREIGN KEY([LogMetadataCourseInstanceId])
REFERENCES [dbo].[LogMetadataCourseInstance] ([LogMetadataCourseInstanceId])
GO

ALTER TABLE [dbo].[LogMetadataAgentInCourseInstance] CHECK CONSTRAINT [FK_LogMetadataAgentInCourseInstance_LogMetadataCourseInstance]
GO

ALTER TABLE [dbo].[LogMetadataAgentInCourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_LogMetadataAgentInCourseInstance_LogMetadataCourseProgramme] FOREIGN KEY([LogMetadataCourseProgrammeId])
REFERENCES [dbo].[LogMetadataCourseProgramme] ([LogMetadataCourseProgrammeId])
GO

ALTER TABLE [dbo].[LogMetadataAgentInCourseInstance] CHECK CONSTRAINT [FK_LogMetadataAgentInCourseInstance_LogMetadataCourseProgramme]
GO




CREATE TABLE [dbo].[LogAgentDashboardSession](
	[LogAgentDashboardSessionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](50) NOT NULL,
	[LogAgentId] [bigint] NULL,
	[Timestamp] [datetime2](7) NULL,
	[ExpirationTimestamp] [datetime2](7) NULL,
 CONSTRAINT [PK_LogAgentDashboardSession] PRIMARY KEY CLUSTERED 
(
	[LogAgentDashboardSessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE dbo.LogMetadataActivityInCourse ADD
	ActivityType nvarchar(50) NULL,
	ObjectId nvarchar(50) NULL
	
	
GO
CREATE NONCLUSTERED INDEX [IX_LogActivity_2]
ON [dbo].[LogActivity] ([Id])
INCLUDE ([LogActivityId])
GO




CREATE TABLE [dbo].[LogStatementDetails](
	[LogStatementId] [uniqueidentifier] NOT NULL,
	[Timespent] [int] NOT NULL,
	[CorrectedTimespent] [int] NULL,
 CONSTRAINT [PK_LogStatementDetails] PRIMARY KEY CLUSTERED 
(
	[LogStatementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LogStatementDetails]  WITH CHECK ADD  CONSTRAINT [FK_LogStatementDetails_LogStatement] FOREIGN KEY([LogStatementId])
REFERENCES [dbo].[LogStatement] ([LogStatementId])
GO

ALTER TABLE [dbo].[LogStatementDetails] CHECK CONSTRAINT [FK_LogStatementDetails_LogStatement]
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION GetMinimum
(
	-- Add the parameters for the function here
	@Param1 Integer, @Param2 Integer
)
RETURNS  int
AS
BEGIN
	-- Declare the return variable here
--	DECLARE @R int

	-- Add the T-SQL statements to compute the return value here
--	SELECT <@ResultVar, sysname, @Result> = <@Param1, sysname, @p1>

	-- Return the result of the function
	RETURN (Select Case When @Param1 < @Param2 
                   Then @Param1 Else @Param2 End MinValue)

END
GO



