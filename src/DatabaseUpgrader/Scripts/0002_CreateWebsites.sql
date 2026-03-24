CREATE TABLE "Websites" (
    "Id" uuid NOT NULL,
    "Url" text NOT NULL,
    "Title" text NOT NULL,
    "Description" text NOT NULL,
    "SubmittedById" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Websites" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Websites_Users_SubmittedById" FOREIGN KEY ("SubmittedById") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Websites_Url" ON "Websites" ("Url");
CREATE INDEX "IX_Websites_SubmittedById" ON "Websites" ("SubmittedById");