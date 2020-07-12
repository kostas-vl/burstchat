export const environment = {
    production: true,
    apiUrl: 'http://localhost:5000',
    signalUrl: 'http://localhost:5001',
    identityUrl: 'http://localhost:5002',
    asteriskUrl: 'localhost:5080',
    clientId: 'burstchat.web.client',
    clientSecret: 'secret',
    scope: 'openid profile offline_access burstchat.api burstchat.signal',
    passwordGrantType: 'password',
    refreshTokenGrantType: 'refresh_token'
};
