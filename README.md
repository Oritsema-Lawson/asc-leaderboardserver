# Ashesi Programming Club (APC) Server

Headless, scalable ASP.NET Core backend designed and written from scratch for general use by the Ashesi Programming Club. Currently, it only works for tracking and maintaining of a small leaderboard based on Codewars Kata completions.

## Features so far

*   Automated codewars API polling.
*   Cookie based authentication.
*   Role-based access control.
*   Argon2id hashing for storage (fully PHC compliant).

---

## API Reference

### Authentication (`/api/Auth`)

#### `POST /api/Auth/register`

Registers a new account. (Defauls to the `UNSPECIFIED` role and must be elevated manually in the database to be able to login).

*   **Payload:**
    ```json
    {
      "userEmail": "someone@example.com",
      "password": "password123"
    }
    ```
*   **Responses:**
    *   `200 OK`: Account successfully created.
    *   `409 Conflict`: User email already exists.

#### `POST /api/Auth/login`

Authenticates a user and issues an authorization cookie.

*   **Payload:**
    ```json
    {
      "userEmail": "hello@example.com",
      "password": "hello123"
    }
    ```
*   **Responses:**
    *   `200 OK`: Login successful.
    *   `401 Unauthorized`: Unknown user, incorrect password, or role is still `UNSPECIFIED`.

#### `POST /api/Auth/logout`

Revokes the current session cookie.

*   **Responses:**
    *   `200 OK`: Cookie successfully deleted.

---

### Admin Routes (`/api/Admin`)

*All routes here require an active session cookie with the `ADMIN` role claim.*

#### `POST /api/Admin/members`

Adds a new Codewars member to the tracking database (will most likely remove points from the constructor but for now its required or the request will fail).

*   **Payload:**
    ```json
    {
      "username": "codewarsUsername",
      "points": 0
    }
    ```
*   **Responses:**
    *   `200 OK`: Member successfully added.
    *   `409 Conflict`: Member already exists in the tracking list.
    *   `401/403`: Not logged in/missing admin role.

#### `POST /api/Admin/katas`

Registers a new Kata to track and assign points for.

*   **Payload:**
    ```json
    {
      "kataRemoteId": "5277c8a221e209d3f6000b56",
      "kataName": "Valid Braces",
      "kataUrl": "https://www.codewars.com/kata/5277c8a221e209d3f6000b56",
      "pointDifficulty": 50
    }
    ```
*   **Responses:**
    *   `200 OK`: Kata successfully added.
    *   `409 Conflict`: Kata already exists.
    *   `401/403`: Not logged in/missing admin role.

#### `DELETE /api/Admin/members/{username}`

Removes a tracked member from the database.

*   **URL Parameter:** `username` (string)
*   **Responses:**
    *   `200 OK`: User successfully deleted.
    *   `404 Not Found`: User not found.
    *   `401/403`: Not logged in/missing admin role.

#### `DELETE /api/Admin/katas/{kataRemoteId}`

Removes a tracked Kata from the database.

*   **URL Parameter:** `kataRemoteId` (string)
*   **Responses:**
    *   `200 OK`: Kata successfully deleted.
    *   `404 Not Found`: Kata not found.
    *   `401/403`: Not logged in/missing admin role.

---

### Open Endpoints (`/api/Main`)

#### `GET /api/Main/leaderboard`

Retrieves the top 20 ranked members based on points.

*   **Responses:**
    *   `200 OK`: Returns a JSON array of member objects.
    ```json
    [
      {
        "username": "codewarsUsername",
        "points": 0
      }
    ]
    ```

#### `GET /api/Main/katas`

Retrieves the list of all currently tracked Katas.

*   **Responses:**
    *   `200 OK`: Returns an array of katas.
    ```json
    [
      {
        "kataRemoteId": "5277c8a221e209d3f6000b56",
        "kataName": "Valid Braces",
        "kataUrl": "https://www.codewars.com/...",
        "pointDifficulty": 50
      }
    ]
    ```
