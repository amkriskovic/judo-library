<template>
  <!-- v-app is root of application | mount point -->
  <v-app dark>

    <!-- "Header" bar -->
    <v-app-bar app dense>

      <!-- Enabling navigation via nuxt-link -> home page, vuetify class -->
      <nuxt-link class="text-h5 text--primary" style="text-decoration: none;" to="/">Judo Library</nuxt-link>

      <v-spacer></v-spacer>

      <!-- Moderation button, if he is Mod -> coming from auth.js getters -->
      <v-btn class="mx-1" v-if="moderator" depressed to="/moderation">Moderation</v-btn>

      <!-- Skeleton loader -> loading animation for content creation -->
      <v-skeleton-loader class="mx-1" :loading="loading" transition="scale-transition" type="button">
        <!-- content-creation-dialog component - Popup -->
        <content-creation-dialog/>
      </v-skeleton-loader>

      <v-skeleton-loader :loading="loading" transition="scale-transition" type="button">
        <!-- If we are authenticated => show Profile -->
        <v-btn depressed outlined v-if="authenticated">
          <v-icon outlined left>mdi-account-circle</v-icon>
          Profile
        </v-btn>

        <!-- Login / Logout section -->
        <!-- Else => not authenticated => Sign in => redirect to our client plugin -->
        <v-btn depressed outlined v-else @click="$auth.signinRedirect()">
          <v-icon outlined left>mdi-account-circle-outline</v-icon>
          Sign in
        </v-btn>
      </v-skeleton-loader>

      <!-- Logout, display only if we are authenticated => means we are logged in => redirect to our client plugin -->
      <v-btn v-if="authenticated" depressed class="ml-1" @click="$auth.signoutRedirect()">Logout</v-btn>

    </v-app-bar>

    <!-- Sizes your content based upon application components -->
    <v-main>
      <v-container>
        <!-- nuxt is where pages are displayed -->
        <nuxt/>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import ContentCreationDialog from "../components/content-creation/content-creation-dialog";
import {mapGetters, mapState} from "vuex";

export default {
  name: "Default",

  // Mapping components
  components: {
    ContentCreationDialog
  },

  // Mapping state & getters from auth.js store
  computed: {
    ...mapState('auth', ['loading']),
    ...mapGetters('auth', ['authenticated', 'moderator']),
  },

}
</script>
