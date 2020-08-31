CREATE TYPE [dbo].[stampoverdraftdetailtabletype] AS TABLE (
    [ID]                             UNIQUEIDENTIFIER NOT NULL,
    [OverdraftID]                    UNIQUEIDENTIFIER NOT NULL,
    [ConceptPaymentID]               UNIQUEIDENTIFIER NOT NULL,
    [Value]                          DECIMAL (18, 6)  NOT NULL,
    [Amount]                         DECIMAL (18, 6)  NOT NULL,
    [Taxed]                          DECIMAL (18, 6)  NOT NULL,
    [Exempt]                         DECIMAL (18, 6)  NOT NULL,
    [IMSSTaxed]                      DECIMAL (18, 6)  NOT NULL,
    [IMSSExempt]                     DECIMAL (18, 6)  NOT NULL,
    [IsGeneratedByPermanentMovement] BIT              NOT NULL);

