<template>
  <!-- v-app is root of application | mount point -->
  <v-app dark>

    <!-- "Header" bar -->
    <v-app-bar app dense>
      <v-toolbar-title>Judo Library</v-toolbar-title>

      <v-spacer></v-spacer>

      <!-- content-creation-dialog component - Popup -->
      <content-creation-dialog/>

    </v-app-bar>

    <!-- Sizes your content based upon application components -->
    <v-main>
      <!-- nuxt is where pages are displayed -->
      <nuxt/>
    </v-main>
  </v-app>
</template>

<script>
  import ContentCreationDialog from "../components/content-creation/content-creation-dialog";

  export default {
    name: "Default",
    // Mapping components
    components: {
      ContentCreationDialog
    }
  }
</script>
