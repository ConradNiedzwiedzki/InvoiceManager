# InvoiceManager

* Project created for a friend. Still in progress. Probably will update this readme if it will be ready.

# How to build/run:

* Set password with the Secret Manager tool: `dotnet user-secrets set SeedUserPW <pw>`. Important: remember about complexity of the password - use capital, special letters and numbers. Without such password, your user manager won't create an admin user and you will get an error while seeding the database.
  
* Update the database:
  `dotnet ef database update`

* Enable HTTPS in the project
