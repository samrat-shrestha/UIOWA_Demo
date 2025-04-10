export const environment = {
  production: true,
  apiUrl: '/api', // This will use the nginx proxy to route to the backend
  oktaConfig: {
    issuer: 'https://dev-94046073.okta.com/oauth2/default',
    clientId: '0oao7rc8ijj6UBKfI5d7',
    redirectUri: 'http://localhost:4200/login/callback',
    scopes: ['openid', 'profile', 'email']
  }
};
