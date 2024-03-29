// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
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

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
