<template>
  <!-- If reverse is true, apply this class -->
  <div class="d-flex align-center" :class="{'flex-row-reverse': reverse}">
    <div :class="{pointer: link}" @click="gotoUser" class="d-flex align-center">
      <!-- Avatar/Img section-->
      <v-avatar :size="size">
        <img v-if="imageUrl" :src="imageUrl"/>
        <v-icon v-else>mdi-account</v-icon>
      </v-avatar>

      <!-- Username section -->
      <div class="mx-3" v-if="username">
        {{ username }}
      </div>
    </div>

    <v-spacer/>

    <slot name="append"></slot>
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

    reverse: {
      required: false,
      type: Boolean,
      default: false
    },

    link: {
      required: false,
      type: Boolean,
      default: true
    }
  },

  methods: {
    gotoUser() {
      if (this.link) {
        return this.$router.push(`/profile/${this.username}`)
      }
    }
  }

}
</script>

<style scoped>

</style>
