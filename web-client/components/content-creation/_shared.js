﻿// * Mixin for shared functionality * //

import {mapActions, mapState} from 'vuex';
import {EVENTS} from "@/data/events";

export const close = {
  methods: {
    ...mapActions("content-creation", ["cancelUpload"]),

    // Wrapping up cancelUpload function into close func
    close() {
      return this.cancelUpload({hard: true});
    }
  }
}


export const form = (formFactory) => ({
  data: () => ({
    // Execute formFactory and assign it to form prop in form function => acts like generic
    form: formFactory()
  }),

  created: function () {
    // Call this setup which comes from content-creation, with this form that's generated with formFactory
    if (this.setup) {
      this.setup(this.form)
    }
  },

  methods: {
    broadcastUpdate() {
      this.$nuxt.$emit(EVENTS.CONTENT_UPDATED)
      this.loadContent()
    },

    ...mapActions('library', ['loadContent'])
  },

  computed: {
    ...mapState("content-creation", ["setup"])
  }
})

