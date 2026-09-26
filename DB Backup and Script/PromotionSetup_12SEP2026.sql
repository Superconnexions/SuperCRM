/*
    SuperCRM - Agent Promotion View Tracking
    Purpose:
      - Agent-specific NEW/viewed behavior for active promotions.
      - One row per Agent user + Promotion.

    Apply to the target database before deploying the application code.
*/

IF OBJECT_ID(N'dbo.AgentPromotionViews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AgentPromotionViews
    (
        AgentPromotionViewId UNIQUEIDENTIFIER NOT NULL,
        PromotionId UNIQUEIDENTIFIER NOT NULL,
        AgentUserId UNIQUEIDENTIFIER NOT NULL,
        ViewedAt DATETIME2 NOT NULL,

        CONSTRAINT PK_AgentPromotionViews
            PRIMARY KEY (AgentPromotionViewId),

        CONSTRAINT FK_AgentPromotionViews_PromotionSetup
            FOREIGN KEY (PromotionId)
            REFERENCES dbo.PromotionSetup (PromotionId),

        CONSTRAINT FK_AgentPromotionViews_AspNetUsers
            FOREIGN KEY (AgentUserId)
            REFERENCES dbo.AspNetUsers (Id)
    );

    CREATE UNIQUE INDEX UQ_AgentPromotionViews_AgentUser_Promotion
        ON dbo.AgentPromotionViews (AgentUserId, PromotionId);

    CREATE INDEX IX_AgentPromotionViews_PromotionId
        ON dbo.AgentPromotionViews (PromotionId);
END;
GO
