describe('CreateUser Page', () => {
  const API_BASE_URL = Cypress.env('REACT_APP_EVENTS_API_BASE_URL');

  beforeEach(() => {
    cy.intercept('GET', `${API_BASE_URL}/api/User/is-admin`, {
      statusCode: 200,
      body: true,
    }).as('getAdminStatus');

    cy.visit('/user/register');
  });

  it('should render form elements', () => {
    cy.get('input[name="UserName"]').should('exist');
    cy.get('input[name="Email"]').should('exist');
    cy.get('input[name="Password"]').should('exist');
    cy.get('button[type="submit"]').should('contain', 'REGISTER');
  });

  it('should submit the form successfully', () => {
    cy.intercept('POST', `${API_BASE_URL}/api/User/CreateAdmin`, {
      statusCode: 200,
      body: { message: 'Account created successfully' },
    }).as('createUserSuccess');

    cy.get('input[name="UserName"]').type('TestUser');
    cy.get('input[name="Email"]').type('testuser@example.com');
    cy.get('input[name="Password"]').type('password123');

    cy.get('button[type="submit"]').click();

    cy.wait('@createUserSuccess');

    cy.get('p').should('contain', 'Account created successfully');
    cy.url().should('include', '/user/login');
  });

  it('should handle errors in form submission', () => {
    cy.intercept('POST', `${API_BASE_URL}/api/User/CreateAdmin`, {
      statusCode: 400,
      body: { errors: [{ description: 'Email is already in use' }] },
    }).as('createUserFailure');

    cy.get('input[name="UserName"]').type('TestUser');
    cy.get('input[name="Email"]').type('existinguser@example.com');
    cy.get('input[name="Password"]').type('password123');

    cy.get('button[type="submit"]').click();

    cy.wait('@createUserFailure');

    cy.get('p').should('contain', 'Error: Email is already in use');
  });

  it('should validate form input fields', () => {
    cy.get('button[type="submit"]').click();

    cy.get('input[name="UserName"]').then(($input) => {
      expect($input[0].validity.valid).to.be.false;
    });
    cy.get('input[name="Email"]').then(($input) => {
      expect($input[0].validity.valid).to.be.false;
    });
    cy.get('input[name="Password"]').then(($input) => {
      expect($input[0].validity.valid).to.be.false;
    });

    cy.get('input[name="Email"]').type('invalid-email');
    cy.get('button[type="submit"]').click();
    
    cy.get('input[name="Email"]').then(($input) => {
      expect($input[0].validity.valid).to.be.false;
    });
  });
});
