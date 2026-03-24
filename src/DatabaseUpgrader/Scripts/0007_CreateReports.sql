CREATE TABLE IF NOT EXISTS "Reports" (
    "Id" uuid NOT NULL,
    "WebsiteId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Reason" text NOT NULL,
    "IsResolved" boolean NOT NULL DEFAULT false,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Reports" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Reports_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Reports_Websites_WebsiteId" FOREIGN KEY ("WebsiteId") REFERENCES "Websites" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Reports_UserId" ON "Reports" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Reports_WebsiteId" ON "Reports" ("WebsiteId");
CREATE INDEX IF NOT EXISTS "IX_Reports_IsResolved" ON "Reports" ("IsResolved");
