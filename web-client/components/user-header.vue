<template>
  <!-- If reverse is true, apply this class -->
  <div class="d-flex align-center pa-2" :class="{'flex-row-reverse': reverse}">
    <!-- Avatar/Img section-->
    <v-avatar :size="size">
      <img v-if="imageUrl" :src="imageUrl"/>
      <v-icon v-else>mdi-account</v-icon>
    </v-avatar>

    <!-- Username section -->
    <div class="mx-3" v-if="username">
      {{ username }}
    </div>

    <v-spacer/>

    <div v-if="append">
      {{ append }}
    </div>
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
    },

    append: {
      required: false,
      type: String,
      default: ''
    },

    reverse: {
      required: false,
      type: Boolean,
      default: false
    }
  }
}
</script>

<style scoped>

</style>
