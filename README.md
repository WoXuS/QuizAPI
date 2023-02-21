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

To create a new user account, use `/api/account/register`. Registration will yield an email confirmation token that should be used at `/api/account/confirmEmail` to validate the account, as it will be impossible to sign up to an account with its email address unconfirmed. Should it be necessary, the email confirmation token can also be retrieved under `/api/account/emailConfirmationToken`. Any subsequent changes of the account's email address will require the same steps.

# Remarks
Most endpoints are secured. From `/api/account`, only the following are public:
- `/api/account/register`
- `/api/account/authenticate`
- `/api/account/refresh`
- `/api/account/username`

The `/api/admin/log` is restricted to the admin account. It provides the administrator with the log of authentication events that occur in the app.

Every endpoint under `api/quizzes` except `/info` is secured. The public one can be used to provide basic information about a quiz to unsigned users. The rest are for manipulating quizzes belonging to the logged in user. Quizzes are private by default - `/open` makes a quiz public, allowing other users to attempt solving it, and `/close` does the opposite. The attempts are shared with the quiz's owner - the owner can inspect the results using `/results`

All endpoints under `api/questions` and `api/answers` are meant for editing the user's quizzes. When a quiz is open, it is impossible to change it or its content. If it is attempted, the API returns `409: Conflict`.

Users can make attempts at solving open quizzes that belong to any user, provided that they know the quiz's ID - quiz owners are expected to share it with their chosen audience through the means of their choice. Before attempting, anyone can use the ID to check the quiz's details under the public `api/quizzes/{id}/info`. To make an attempt, use POST at `api/attempts` - it will result in a creation of a new attempt that will be open to modification by the attempting user. The attempt includes a copy of the quiz - use GET to inspect it. Use PATCH to mark the answers you deem correct. When you're done, use `/api/attempts/{id}/close` to finish the attempt. Should you want to abandon the attempt, you can DELETE it as long as it has not been closed. To view your results, use `/api/attempts/{id}/result`. There is no limit to the number of attempts a user can make at the same quiz.
