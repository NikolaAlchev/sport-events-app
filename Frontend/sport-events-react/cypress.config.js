const { defineConfig } = require("cypress");

module.exports = defineConfig({
  e2e: {
    baseUrl: "http://localhost:3000",
    supportFile: false,
    env: {
      REACT_APP_MATCHES_API_BASE_URL: "http://localhost:5260",
      REACT_APP_EVENTS_API_BASE_URL: "https://localhost:7023"
    }
  }
});
