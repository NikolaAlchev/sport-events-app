describe('Team Page', () => {
  const teamId = 61;
  const apiUrl = `${Cypress.env('REACT_APP_MATCHES_API_BASE_URL')}/team/${teamId}`;

  beforeEach(() => {
      cy.intercept('GET', apiUrl, {
          fixture: 'team.json',
      }).as('getTeam');
      cy.visit(`/team/${teamId}`);
  });

  it('should display a loader initially', () => {
      cy.get('[data-testid="loader"]').should('exist');
  });

  it('should display team information after loading', () => {
      cy.wait('@getTeam');
      cy.get('[data-testid="team-name"]').should('contain', 'Chelsea FC');
      cy.get('[data-testid="team-venue"]').should('contain', 'Stamford Bridge');
      cy.get('[data-testid="team-crest"]').should('be.visible');
  });

  it('should handle API errors gracefully', () => {
      cy.intercept('GET', apiUrl, {
          statusCode: 500,
      }).as('getTeamError');
      cy.visit(`/team/${teamId}`);
      cy.wait('@getTeamError');
      cy.get('[data-testid="error-message"]').should('be.visible');
  });

  it('should switch tabs correctly', () => {
      cy.wait('@getTeam');
      cy.get('[data-testid="tab-schedule"]').click();
      cy.get('[data-testid="schedule-content"]').should('be.visible');
      
      cy.get('[data-testid="tab-players"]').click();
      cy.get('[data-testid="players-content"]').should('be.visible');
  });

  it('should display past matches in the profile tab', () => {
      cy.wait('@getTeam');
      cy.get('[data-testid="profile-content"]').should('contain', 'Premier League');
  });

  it('should navigate to a match page when a match is clicked', () => {
      cy.wait('@getTeam');
      cy.get('[data-testid="match-row"]').first().click();
      cy.url().should('include', '/matches/');
  });
});