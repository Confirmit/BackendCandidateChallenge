CREATE TABLE [Quiz](
    [Id] INTEGER NOT NULL,
    [Title] [nvarchar](256) NOT NULL,
    CONSTRAINT [PK_Quiz] PRIMARY KEY ([Id] ASC)
);

CREATE TABLE [Question](
    [Id] INTEGER NOT NULL,
    [Text] [nvarchar](256) NOT NULL,
    [QuizId] [int] NOT NULL,
    [CorrectAnswerId] [int] NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY ([Id] ASC),
    CONSTRAINT [FK_Quiz_Question] FOREIGN KEY([QuizId]) REFERENCES [Quiz] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Answer_Question] FOREIGN KEY([CorrectAnswerId]) REFERENCES [Answer] ([Id])
);

CREATE TABLE [Answer](
    [Id] INTEGER NOT NULL,
    [Text] [nvarchar](256) NOT NULL,
    [QuestionId] [int] NOT NULL,
    CONSTRAINT [PK_Answer] PRIMARY KEY ([Id] ASC),
    CONSTRAINT [FK_Question_Answer] FOREIGN KEY([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [QuizResponse](
	[Id] INTEGER NOT NULL PRIMARY KEY,
	[QuizId] [integer] NOT NULL,
	[QuestionId] [integer] NOT NULL,
	[AnswerId] [integer] NOT NULL,
	[UserId] [integer] NOT NULL
);