CREATE TABLE "WebsiteCategories" (
    "WebsiteId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    CONSTRAINT "PK_WebsiteCategories" PRIMARY KEY ("WebsiteId", "CategoryId"),
    CONSTRAINT "FK_WebsiteCategories_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_WebsiteCategories_Websites_WebsiteId" FOREIGN KEY ("WebsiteId") REFERENCES "Websites" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_WebsiteCategories_CategoryId" ON "WebsiteCategories" ("CategoryId");