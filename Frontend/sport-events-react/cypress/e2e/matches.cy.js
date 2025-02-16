describe("Matches Page", () => {
  beforeEach(() => {
    cy.intercept("GET", `${Cypress.env("REACT_APP_MATCHES_API_BASE_URL")}/matches/all*`, {
      fixture: "matches.json",
    }).as("getMatches");

    cy.visit("/matches");
  });

  it("should display the Matches page correctly", () => {
    cy.contains("Select a match").should("be.visible");
    cy.get(".league-list").should("exist");
  });

  it("should fetch and display matches", () => {
    cy.wait("@getMatches").its("response.statusCode").should("eq", 200);

    cy.get(".league-list").children().should("have.length.above", 0);

    cy.get(".league-list").find("[data-test='match-row']").should("exist");
  });

  it("should group matches by league", () => {
    cy.get(".league-list").find("[data-test='league-container']").should("have.length.above", 0);
  });

  it("should navigate to match details when clicking a match", () => {
    cy.get("[data-test='match-row']").first().click();
    cy.url().should("include", "/matches/");
  });

  it("should allow date selection and update matches", () => {
    cy.wait("@getMatches");

    cy.get('input[type="date"]').as("datePicker");

    cy.get("@datePicker").invoke("val").then((currentDate) => {
      const newDate = new Date();
      newDate.setDate(newDate.getDate() + 1);
      const newDateStr = newDate.toISOString().split("T")[0];

      if (newDateStr !== currentDate) {
        cy.get("@datePicker").clear().type(newDateStr);
        cy.wait("@getMatches");
        cy.get(".league-list").should("exist");
      }
    });
  });
});
