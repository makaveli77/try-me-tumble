CREATE TABLE "Upvotes" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "WebsiteId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Upvotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Upvotes_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Upvotes_Websites_WebsiteId" FOREIGN KEY ("WebsiteId") REFERENCES "Websites" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Upvotes_UserId" ON "Upvotes" ("UserId");
CREATE INDEX "IX_Upvotes_WebsiteId" ON "Upvotes" ("WebsiteId");