<template>
  <!-- v-app is root of application | mount point -->
  <v-app dark>

    <!-- "Header" bar -->
    <v-app-bar app>

      <!-- Enabling navigation via nuxt-link -> home page, vuetify class -->
      <nuxt-link class="text-h5 text--primary mr-2" style="text-decoration: none;" to="/">
        <span class="d-none d-md-flex">Judo Library</span>
        <span class="d-flex d-md-none">TL</span>
      </nuxt-link>

      <v-spacer/>
      <nav-bar-search/>
      <v-spacer/>

      <!-- If we are authenticated (if-auth component handles logic), display content-creation-dialog && Profile -->
      <if-auth>
        <template v-slot:allowed="{moderator}">
          <div class="d-flex align-center">
            <!-- Moderation button, if he is Mod -> coming from auth.js getters -->
            <v-btn v-if="moderator" class="d-none d-md-flex mx-1" depressed to="/moderation">Moderation</v-btn>

            <content-creation-dialog/>

            <!-- Dropdown - Menu -->
            <v-menu offset-y>
              <template v-slot:activator="{ on, attrs }">
                <v-btn icon v-bind="attrs" v-on="on">
                  <user-header :image-url="profile.image" :link="false" size="36"/>
                </v-btn>
              </template>
              <v-list>

                <!-- Moderation -->
                <v-list-item class="d-flex d-md-none" to="/moderation">
                  <v-list-item-title>
                    <v-icon>mdi-clipboard</v-icon>
                    Moderation
                  </v-list-item-title>
                </v-list-item>

                <!-- Profile -->
                <v-list-item @click="$router.push('/profile')">
                  <v-list-item-title>
                    <v-icon>mdi-account-circle</v-icon>
                    Profile
                  </v-list-item-title>
                </v-list-item>

                <!-- Logout, means we are logged in => redirect to our client plugin -->
                <v-list-item @click="logout">
                  <v-list-item-title>
                    <v-icon outlined left>mdi-logout</v-icon>
                    Logout
                  </v-list-item-title>
                </v-list-item>

              </v-list>
            </v-menu>
          </div>
        </template>

        <!-- If we are not authenticated => Forbidden -->
        <template v-slot:forbidden="{login}">
          <!-- Login / Logout section -->
          <!-- Else => not authenticated => Sign in => redirect to our client plugin =>> login -->
          <v-btn outlined @click="login">
            <v-icon outlined left>mdi-account-circle-outline</v-icon>
            Log in
          </v-btn>
        </template>
      </if-auth>
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
import {mapActions, mapGetters, mapState} from "vuex";
import IfAuth from "@/components/auth/if-auth";
import NavBarSearch from "@/components/nav-bar-search";
import UserHeader from "@/components/user-header";

export default {
  name: "default",

  // Mapping components
  components: {
    UserHeader,
    NavBarSearch,
    IfAuth,
    ContentCreationDialog
  },

  // Mapping state & getters from auth.js store
  computed: {
    ...mapState('auth', ['profile']),
    ...mapGetters('auth', ['moderator']),
  },

  methods: mapActions('auth', ['logout'])
}
</script>
