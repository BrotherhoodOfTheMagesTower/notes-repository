/// <reference types="cypress" />

const noteContent = "#Test note content"
const noteTitle = "#Test note title"

describe('User is able to add a note with default title to default directory', () => {
  before(() => {
    cy.visit('Identity/Account/Login')
    cy.get('[data-ref="username"]').type('note@1.com')
    cy.get('[data-ref="password"]').type('Admin123!')
    cy.get('[data-ref="login"]').click()
    cy.visit('editNote/newNote')
    //call GET endpoint
  })

  it('displays the content input and `save` button', () => {
    cy.get('[data-ref="note-input"]').should('be.visible')
    cy.get('[data-ref="save-note"]').should('be.visible')
  })

  it('is possible to add a note with default title, directory and emoji', () => {
    cy.get('[data-ref="note-input"]').type(noteContent)
    cy.get('[data-ref="save-note"]').click()
    cy.get('[id="noteTitle"]').should('have.value', 'New note')
    cy.xpath('//select[@id="noteDirecotry"]/option').first().should('have.value', 'Default')
    cy.xpath('//div[@class="blazored-modal-content"]//button[text()="Save"]').click()
    cy.xpath('//*[@class="blazored-toast-message"]', { timeout: 10000 })
      .should("have.text", "The note was saved.")
    cy.xpath('//*[@class="blazored-toast-close-icon"]').click()
  })

  after(() => {
    //call DELETE endpoint
  })
})

describe('User is not able to add a note with empty content', () => {
  before(() => {
    cy.visit('Identity/Account/Login')
    cy.get('[data-ref="username"]').type('note@note.com')
    cy.get('[data-ref="password"]').type('Admin123!')
    cy.get('[data-ref="login"]').click()
    cy.visit('editNote/newNote')
    //call GET endpoint
  })

  it('is not possible to add a note with empty content', () => {
    cy.get('[data-ref="save-note"]').click()
    cy.xpath('//*[@class="blazored-toast-message"]', { timeout: 10000 })
      .should("have.text", "Your note is empty.")
  })

  after(() => {
    //call DELETE endpoint
  })
})

describe('User is not able to add a note without title', () => {
  before(() => {
    cy.visit('Identity/Account/Login')
    cy.get('[data-ref="username"]').type('note@1.com')
    cy.get('[data-ref="password"]').type('Admin123!')
    cy.get('[data-ref="login"]').click()
    cy.visit('editNote/newNote')
    //call GET endpoint
  })

  it('is not possible to add a note without title', () => {
    cy.get('[data-ref="note-input"]').type(noteContent)
    cy.get('[data-ref="save-note"]').click()
    cy.get('[id="noteTitle"]').clear()
    cy.xpath('//div[@class="blazored-modal-content"]//button[text()="Save"]').click()
    cy.xpath('//*[@class="validation-message"]')
      .should("have.text", "Required field!")
  })

  after(() => {
    //call DELETE endpoint
  })
})

describe('User is not able to add a note with a title, which has 1 character', () => {
  before(() => {
    cy.visit('Identity/Account/Login')
    cy.get('[data-ref="username"]').type('note@1.com')
    cy.get('[data-ref="password"]').type('Admin123!')
    cy.get('[data-ref="login"]').click()
    cy.visit('editNote/newNote')
    //call GET endpoint
  })

  it('is not possible to add a note with title, which has only 1 character', () => {
    cy.get('[data-ref="note-input"]').type(noteContent)
    cy.get('[data-ref="save-note"]').click()
    cy.get('[id="noteTitle"]').clear().type('t{enter}')
    cy.xpath('//*[@class="validation-message"]')
      .should("have.text", "Too short!")
  })

  after(() => {
    //call DELETE endpoint
  })
})

describe('User is not able to check, when the note was edited', () => {
  // before(() => {
  //   cy.visit('Identity/Account/Login')
  //   cy.get('[data-ref="username"]').type('note@1.com')
  //   cy.get('[data-ref="password"]').type('Admin123!')
  //   cy.get('[data-ref="login"]').click()
  //   cy.visit('editNote/newNote')
  //   //call GET endpoint
  // })

  // it('is not possible to add a note with title, which has only 1 character', () => {
  //   cy.get('[data-ref="note-input"]').type(noteContent)
  //   cy.get('[data-ref="save-note"]').click()
  //   cy.get('[id="noteTitle"]').clear().type('t{enter}')
  //   cy.xpath('//*[@class="validation-message"]')
  //     .should("have.text", "Too short!")
  // })

  // after(() => {
  //   //call DELETE endpoint
  // })
})
