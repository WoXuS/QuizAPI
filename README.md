# QuizAPI
A .NET7 web API for handling quizzes.

# Setup
- By default, the SQL Server LocalDB that comes with Visual Studio is used. Can be changed in `appsettings.json` as `ConnectionStrings:QuizApiDb` value.
- Run Update-Database in Visual Studio's Package Manager Console.
- Modify the `JwtSettings:Secret` value in `appsettings.json` to secure the API access.
- Run the app from Visual Studio.

# Usage
Launching the app should open Swagger UI in your browser with which you can test the API. The app comes with a default admin account `(username: admin, password: admin)` - change the credentials if you care about security. To do so, first use the `/api/account/authenticate` to sign in as the admin - in the response you will receive the JWT token (along with its corresponding refresh token) with which you can access the API's secured endpoints.

After getting the token, use the `Authorize` button in the top-right corner and follow the instructions. For example, having received a token `my_t0k3n`, enter the following text into the input field: `bearer my_t0k3n`. Confirm with the `Authorize` button below and close the popup.

Now you can access the secured endpoints. Try `/api/account/details` to check the admin account's details. Use `/api/account/credentials` to change the admin password.

In a short period, the access token you received will become outdated and you will no longer be authenticated (the token lifetime can be configured under `JwtSettings:Lifetime` in `appsettings.json`). To save the effort of typing the credentials again, copy the output you got from `/api/account/authenticate` and use it as input at `/api/account/refresh` - you will receive a new pair of tokens, in which the first one can be used to regain API access (using the `Authorize` button) while the second one is a corresponding refresh token.

To create a new user account, use `/api/account/register`.
