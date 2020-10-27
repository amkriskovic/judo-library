<template>
  <div>
    <!-- Skeleton loader -> loading animation for content creation, If we are authenticated or loading -->
    <v-skeleton-loader v-if="loading || authenticated"
                       class="mx-1"
                       :loading="loading"
                       transition="scale-transition"
                       type="button">

      <slot name="allowed"></slot>
    </v-skeleton-loader>

    <slot v-else name="forbidden" :login="login"></slot>
  </div>
</template>

<script>
import {mapActions, mapGetters, mapState} from "vuex";

export default {
  name: "if-auth",

  computed: {
    ...mapState('auth', ['loading']),
    ...mapGetters('auth', ['authenticated']),
  },

  methods: mapActions('auth', ['login'])
}
</script>

<style scoped>

</style>
