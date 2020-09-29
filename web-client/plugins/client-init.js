// Nuxt.js allows you to define JavaScript plugins to be run before instantiating the root Vue.js Application.
// This is especially helpful when using Vue libraries, external modules or your own plugins.

import {UserManager, WebStorageStateStore} from "oidc-client";

// {app} -> context of our whole Vue application | store -> context of vuex store
export default async ({app, store}, inject) => {

  // Instantiate user manager and provide required setting, as well as some optional settings
  const userManager = new UserManager({
    // The URL of the OIDC/OAuth2 provider.
    authority: "https://localhost:5001",

    // Your client application's identifier as registered with the OIDC/OAuth2 provider.
    client_id: "web-client",

    // The redirect URI of your client application to receive a response from the OIDC/OAuth2 provider.
    redirect_uri: "https://localhost:3000/oidc/sign-in-callback.html",

    // The type of response desired from the OIDC/OAuth2 provider.
    response_type: "code",

    // The scope is being requested from the OIDC/OAuth2 provider.
    // * All the resources hidden behind our API, scope is gonna be specifically for the policy that's protecting it
    // In case od Mod, role scope is gonna be requested -> role: "Mod" => /userinfo
    scope: "openid profile IdentityServerApi role",

    // The OIDC/OAuth2 post-logout redirect URI.
    post_logout_redirect_uri: "https://localhost:3000",

    // The URL for the page containing the code handling the silent renew.
    // * Page where <iframe> will land on, and have correct callback, it will rise correct events in this plugin
    silent_redirect_uri: "https://localhost:3000/oidc/sign-in-silent-callback.html",

    // Storage object used to persist User for currently authenticated user.
    // Saving user to the local storage rather than session storage, so it persists after we close the browser
    // * Session storage is temporary, after we close the browser it's deleted -> loosing authentication
    // * Solution => store the tokens in the local storage
    userStore: new WebStorageStateStore({ store: window.localStorage }),
  })

  // * (Key, Value) => injecting userManager in App when we reference with $auth
  inject('auth', userManager)

  // As long our App loads, this fetch fund should execute
  app.fetch = () => {
    // * Actions are triggered with the store.dispatch method => triggering client initialization
    return store.dispatch('clientInit')
  }

}
