import re

with open('/Users/almirhrvat/Desktop/projects/try-me-tumble/src/Application/Services/WebsiteService.cs', 'r') as file:
    content = file.read()

content = re.sub(
    r'(await _redis\.ListRightPushAsync\(\$"websites_list:category:\{websiteDto\.CategoryId\.Value\}", serializedDto\);)\s*\}',
    r'\1\n            }\n            return dto;\n        }',
    content
)

with open('/Users/almirhrvat/Desktop/projects/try-me-tumble/src/Application/Services/WebsiteService.cs', 'w') as file:
    file.write(content)

