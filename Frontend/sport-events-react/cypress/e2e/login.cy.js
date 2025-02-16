describe("Login Page", () => {
  const eventsApiUrl = Cypress.env("REACT_APP_EVENTS_API_BASE_URL");

  beforeEach(() => {
    cy.visit("/user/login");
  });

  it("should display the login form", () => {
    cy.get("input[name='Email']").should("be.visible");
    cy.get("input[name='Password']").should("be.visible");
    cy.get("button[type='submit']").should("contain", "LOGIN");
  });

  it("should show an error for invalid credentials", () => {
    cy.intercept("POST", `${eventsApiUrl}/api/User/Login`, {
      statusCode: 401,
      body: { errors: [{ description: "Invalid email or password" }] },
    }).as("loginRequest");

    cy.get("input[name='Email']").type("wrong@example.com");
    cy.get("input[name='Password']").type("wrongpassword{enter}");

    cy.wait("@loginRequest");
    cy.contains("Error: Invalid email or password").should("be.visible");
  });

  it("should successfully log in with valid credentials", () => {
    cy.intercept("POST", `${eventsApiUrl}/api/User/Login`, {
      statusCode: 200,
      body: { message: "Login successful" },
    }).as("loginRequest");

    cy.get("input[name='Email']").type("user@example.com");
    cy.get("input[name='Password']").type("password123{enter}");

    cy.wait("@loginRequest");
    cy.url().should("include", "/matches");
  });
});
