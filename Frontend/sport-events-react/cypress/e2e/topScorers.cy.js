describe('Top Scorers Page', () => {
    beforeEach(() => {
        cy.fixture('topScorers.json').then((topScorers) => {
            cy.intercept('GET', '**/player/*/topScorers', {
                statusCode: 200,
                body: topScorers
            }).as('getTopScorers');
        });

        cy.visit('/player/1');
    });

    it('should display a loading indicator initially', () => {
        cy.get('[data-testid="loader"]').should('be.visible');
    });

    it('should fetch and display top scorers', () => {
        cy.wait('@getTopScorers');

        cy.get('[data-testid="player-card"]').should('have.length', 2);
        cy.get('[data-testid="player-name"]').first().should('contain', 'Lionel Messi');
        cy.get('[data-testid="player-stats"]').first().should('contain', 'Goals: 25');
    });

    it('should display an error message if API fails', () => {
        cy.intercept('GET', '**/player/*/topScorers', {
            statusCode: 500,
            body: {}
        }).as('getTopScorersError');

        cy.visit('/player/1');
        cy.wait('@getTopScorersError');

        cy.get('[data-testid="error-message"]').should('be.visible');
    });
});