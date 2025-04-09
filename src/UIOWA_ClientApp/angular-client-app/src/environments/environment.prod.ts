export const environment = {
  production: true,
  apiUrl: 'https://your-production-api-url/api',
  oktaConfig: {
    issuer: 'https://dev-12345678.okta.com/oauth2/default',
    clientId: 'your-client-id',
    redirectUri: 'https://your-production-url/login/callback',
    scopes: ['openid', 'profile', 'email']
  }
}; 