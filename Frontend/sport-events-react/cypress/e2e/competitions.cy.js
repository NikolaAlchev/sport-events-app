describe('Competitions Page', () => {
    beforeEach(() => {
        cy.fixture('competitions.json').then((competitions) => {
            cy.intercept('GET', '**/competitions/all', {
                statusCode: 200,
                body: competitions
            }).as('getCompetitions');
        });
    });

    it('should display a loading indicator initially', () => {
        cy.visit('/competitions');
        cy.get('[data-testid="loader"]').should('be.visible');
    });

    it('should fetch and display competitions', () => {
        cy.visit('/competitions');
        cy.wait('@getCompetitions');
        
        cy.get('[data-testid="competition-container"]').should('exist');
        cy.get('[data-testid="competition-link"]').should('have.length', 3);
        cy.get('[alt="Premier League"]').should('exist');
        cy.get('[alt="La Liga"]').should('exist');
    });

    it('should display an error message on API failure', () => {
        cy.intercept('GET', '**/competitions/all', {
            statusCode: 500,
            body: {}
        }).as('getCompetitionsError');
        
        cy.visit('/competitions');
        cy.wait('@getCompetitionsError');
        cy.get('[data-testid="error-message"]').should('be.visible');
    });
});