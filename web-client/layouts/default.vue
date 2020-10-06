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

        <!-- If we are authenticated -->
        <!-- Dropdown - Menu -->
        <v-menu offset-y v-if="authenticated">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <!-- Profile avatar image -->
              <v-avatar size="38">
                <!-- Make use of videos controller method for getting the profile image-->
                <img v-if="profile.image" :src="`https://localhost:5001/api/videos/${profile.image}`"
                     alt="profile image"/>

                <v-icon v-else>mdi-account</v-icon>
              </v-avatar>
            </v-btn>
          </template>
          <v-list>
            <!-- Profile -->
            <v-list-item @click="$router.push('/profile')">
              <v-list-item-title>
                <v-icon>mdi-account-circle</v-icon>
                Profile
              </v-list-item-title>
            </v-list-item>

            <!-- Logout, means we are logged in => redirect to our client plugin -->
            <v-list-item @click="$auth.signoutRedirect()">
              <v-list-item-title>
                <v-icon outlined left>mdi-logout</v-icon>
                Logout
              </v-list-item-title>
            </v-list-item>

          </v-list>
        </v-menu>

        <!-- Login / Logout section -->
        <!-- Else => not authenticated => Sign in => redirect to our client plugin -->
        <v-btn depressed outlined v-else @click="$auth.signinRedirect()">
          <v-icon outlined left>mdi-account-circle-outline</v-icon>
          Log in
        </v-btn>
      </v-skeleton-loader>

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
    ...mapState('auth', ['loading', 'profile']),
    ...mapGetters('auth', ['authenticated', 'moderator']),
  },

}
</script>
