// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  authApiUrl: 'https://z7qv6ih936.execute-api.eu-west-1.amazonaws.com/prod',
  gamesApiUrl: 'https://pirasy6xq1.execute-api.eu-west-1.amazonaws.com/prod',

  pagination: {
    desktop: 10,
    mobile: 3,
  },
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
