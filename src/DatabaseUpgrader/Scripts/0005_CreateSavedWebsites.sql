CREATE TABLE "SavedWebsites" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "WebsiteId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_SavedWebsites" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_SavedWebsites_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SavedWebsites_Websites_WebsiteId" FOREIGN KEY ("WebsiteId") REFERENCES "Websites" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_SavedWebsites_UserId" ON "SavedWebsites" ("UserId");
CREATE INDEX "IX_SavedWebsites_WebsiteId" ON "SavedWebsites" ("WebsiteId");