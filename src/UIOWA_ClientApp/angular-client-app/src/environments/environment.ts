export const environment = {
  production: false,
  apiUrl: 'http://localhost:5291/api',
  oktaConfig: {
    issuer: 'https://dev-94046073.okta.com/oauth2/default',
    clientId: '0oao7rc8ijj6UBKfI5d7',
    redirectUri: 'http://localhost:4200/login/callback',
    scopes: ['openid', 'profile', 'email']
  }
}; 