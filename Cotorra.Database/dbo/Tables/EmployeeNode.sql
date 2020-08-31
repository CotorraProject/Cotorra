﻿CREATE TABLE [dbo].[EmployeeNode] (
    [ID] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    INDEX [GRAPH_UNIQUE_INDEX_3BC2B343C13D4904BC25490EC3357376] UNIQUE NONCLUSTERED ($node_id)
) AS NODE;
