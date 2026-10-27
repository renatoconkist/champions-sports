# Champions Sports — API Endpoints

> Prepared for handoff to the client's backend team.  
> Base: existing PHP API documented in Notion "Endpoints SportChampions" (~54 endpoints).  
> New screens mapped below; gaps marked **🆕 NEW** (endpoint must be added by backend).  
> All protected endpoints require `Authorization: Bearer <access_token>`.  
> Default locale header: `Accept-Language: pt-BR` (or `en-US` / `es-ES`).

---

## Authentication (`/auth`)

| Method | Path | Screen | Description | Notes |
|--------|------|--------|-------------|-------|
| GET | `/status` | Boot | Server health check | Returns 200 OK if API is up |
| POST | `/login` | Login | Authenticate user | Body: `{ login, password }` → returns `{ access_token, refresh_token, token_type, expires_in }` |
| POST | `/signup` | Cadastro | Create account | Body: `{ email, password, username, phone? }` |
| POST | `/forgot-password` | Esqueci Senha | Trigger password reset email | Body: `{ email }` |
| POST | `/logout` | (any) | Invalidate token | Bearer token required |
| 🆕 NEW | POST `/auth/refresh` | (background) | Refresh access token silently | Body: `{ refresh_token }` → new `access_token` |
| 🆕 NEW | POST `/support` | Suporte | Contact form submission | Body: `{ subject, message, email }` — **not in old API** |

---

## Account (`/account`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| PATCH | `/update-account` | (Profile settings) | Update email, username, phone, password | Body: `{ username?, email?, phone?, password, new_password?, new_password_confirmation? }` |
| DELETE | `/delete-account` | (Settings) | Delete account | Body: `{ password }` + Bearer token |

---

## Profile (`/profile`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/profile` | Home / Onboarding | Returns profile, account info and game progression (level, xp, coins, skill_tree_progress) |
| POST | `/update-profile` | Perfil do Atleta | Update name, age, avatar settings, positions | Body: `{ given_name, last_name, age, avatar_settings, positions[] }` |
| POST | `/profile/positions` | Posições de Jogo | Save up to 3 playing positions | Body: `{ positions: ["Atacante", "Meia", "Goleiro"] }` |
| POST | `/upload-photo` | Perfil do Atleta | Upload avatar photo | Multipart: `key=foto, type=file` |

---

## Home Content (`/content`, `/videos`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/content` | Home Dashboard | Returns featured cards, news, events, challenges, game data |
| GET | `/videos` | Home — Reels carousel | Video list (id + path); thumbnail currently missing → 🆕 request backend to add `thumbnail_url` and `title` |

---

## Training (`/trainings`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/trainings` | Treino | List created trainings |
| POST | `/start-training` | Treino | Start training | Body: `{ training_challenge_id }` |
| POST | `/finish-training` | Treino | Finish training | Body: `{ training_challenge_id }` |

---

## Challenges (`/challenges`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/challenges` | (Missões / Desafios) | List challenges linked to the user's account |
| POST | `/complete-tactic` | (Desafios) | Dispatch event when a tactic challenge is completed |

---

## Reports / Evaluation (`/reports`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/reports` | Avaliação | Load player performance reports |
| POST | `/new-report` | Avaliação | Request re-evaluation |

---

## Subscriptions / Store (`/subscriptions`, `/store`)

| Method | Path | Screen | Description |
|--------|------|--------|-------------|
| GET | `/subscriptions` | (Configurações) | List plans + current active plan |
| POST | `/subscribe` | (Loja) | Execute subscription |
| POST | `/cancel-subscription` | (Configurações) | Cancel subscription |
| GET | `/store-items` | (Loja) | List store items |
| GET | `/cart` | (Loja) | Cart contents |
| POST | `/add-to-cart` | (Loja) | Add item to cart |
| POST | `/update-cart` | (Loja) | Update cart |
| GET | `/checkout` | (Loja) | Payment receipt |
| POST | `/checkout` | (Loja) | Confirm purchase |

---

## Social / Friends (`/friends`, `/chat`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/friends` | Friend list |
| GET | `/friend-requests` | Pending friend requests (sent + received) |
| POST | `/add-friend` | Send friend request — `{ id_friend }` |
| POST | `/accept-friend` | Accept request — `{ id_friend }` |
| POST | `/reject-friend` | Reject request — `{ id_friend }` |
| POST | `/remove-friend` | Remove friend — `{ id_friend }` |
| POST | `/block-account` | Block user — `{ id_account }` |
| POST | `/report-account` | Report user for moderation |
| GET | `/messages` | Inbox messages |
| POST | `/add-chat` | Create 1-on-1 chat channel — `{ id_friend }` |
| POST | `/chat-history` | Load chat messages — `{ chat_id }` |
| POST | `/send-message` | Send message — `{ chat_id, content }` |
| POST | `/edit-message` | Edit message — `{ id_message, content }` |
| DELETE | `/remove-message` | Delete message — `{ id_message }` |
| POST | `/add-group` | Create group — `{ name, account_ids[] }` |
| POST | `/remove-group` | Delete group — `{ chat_id }` |
| POST | `/add-friend-to-group` | Add members — `{ chat_id, account_ids[] }` |
| POST | `/remove-friend-from-group` | Remove member — `{ chat_id, account_id }` |
| POST | `/silence-channel` | Mute channel — `{ chat_id, mute }` |
| POST | `/delete-channel` | Leave or delete channel |
| POST | `/send-video` | Post video challenge — multipart |
| POST | `/send-picture` | Post photo to social feed |
| POST | `/like-post` | Like a post |
| POST | `/comment-post` | Comment on a post |
| GET | `/post-comments` | Load post comments |
| POST | `/post-shared` | Share a post |

---

## Minigames

| Method | Path | Description |
|--------|------|-------------|
| GET | `/minigames` | List available minigames |

---

## 🆕 GAPS — New Endpoints Required

These screens in the new app design need data the old API does not provide.  
The backend team must design and implement these endpoints.

| Priority | Method | Suggested Path | Screen | Data Needed |
|----------|--------|----------------|--------|-------------|
| **HIGH** | GET | `/missions` | Missões | List active/completed missions with rewards |
| **HIGH** | POST | `/missions/{id}/complete` | Missões | Mark mission as complete, trigger reward |
| **HIGH** | GET | `/ranking` | Ranking | Leaderboard rows (position, username, avatar, score, league) |
| **HIGH** | GET | `/league` | Ranking | Current user league, division, promotion threshold |
| **HIGH** | GET | `/wallet` | Home (TopBar coins/XP) | Real-time coins + XP balance (currently nested in /profile only) |
| **HIGH** | POST | `/support` | Suporte | Contact form (subject, message) |
| **MEDIUM** | GET | `/rewards` | Overlay-Reward | Reward history / pending rewards |
| **MEDIUM** | POST | `/auth/refresh` | (background) | Silent token refresh |
| **LOW** | GET | `/skills` | Habilidades | Skill tree nodes, unlocked/locked state |
| **LOW** | GET | `/notifications` | (TopBar bell) | In-app notification list |

---

## Common Conventions

```
Base URL:        https://api.champions-sports.com/v1   (TBC with backend team)
Auth header:     Authorization: Bearer <access_token>
Locale header:   Accept-Language: pt-BR | en-US | es-ES
Content-Type:    application/json  (multipart/form-data for uploads)
Errors:          { "message": "...", "errors": { ... } }   (Laravel convention)
Pagination:      ?page=1&per_page=20  (where applicable)
```

---

## OpenAPI Draft

An OpenAPI 3.0 YAML draft is located at `docs/api/openapi.yaml` (to be generated from this doc).  
Generated with: `npx @openapitools/openapi-generator-cli generate ...`  
Pending: backend team to provide base URL and confirm field names.
