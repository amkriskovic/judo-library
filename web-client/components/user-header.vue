<template>
  <div class="d-flex align-center pa-2">
    <!-- Avatar/Img section-->
    <v-avatar :size="size">
      <img :src="imageUrl"/>
    </v-avatar>

    <!-- Username section -->
    <div class="ml-3" v-if="username">
      {{ username }}
    </div>

    <v-spacer/>
  </div>
</template>

<script>
export default {
  // User component responsible for displaying User profile data
  name: "user-header",

  // Configuring props for User component
  props: {
    imageUrl: {
      required: false,
      type: String
    },

    username: {
      required: false,
      type: String
    },

    // Image/Avatar size
    size: {
      required: false,
      type: String,
      default: '42',
    }
  }
}
</script>

<style scoped>

</style>
