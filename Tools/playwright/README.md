Playwright UI tests for AfyaConnectLite

Setup (requires Node.js >= 16):

1. cd Tools/playwright
2. npm install
3. npx playwright install
4. Start the AfyaConnectLite app (it must listen at http://localhost:5019)
5. Run tests: npm test

Notes:
- Tests run headless by default. To run headed, edit `playwright.config.ts` and set `headless: false`.
- The test registers a new user and attempts to book an appointment via the Blazor UI.
