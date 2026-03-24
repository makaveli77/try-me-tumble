CREATE TABLE IF NOT EXISTS "Tags" (
    "Id" uuid NOT NULL,
    "Name" character varying(50) NOT NULL,
    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Tags_Name" ON "Tags" ("Name");

CREATE TABLE IF NOT EXISTS "WebsiteTags" (
    "WebsiteId" uuid NOT NULL,
    "TagId" uuid NOT NULL,
    CONSTRAINT "PK_WebsiteTags" PRIMARY KEY ("WebsiteId", "TagId"),
    CONSTRAINT "FK_WebsiteTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_WebsiteTags_Websites_WebsiteId" FOREIGN KEY ("WebsiteId") REFERENCES "Websites" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_WebsiteTags_TagId" ON "WebsiteTags" ("TagId");
