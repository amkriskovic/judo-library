<template>
  <!-- Comment input, place where we type in our comment, reusable for comment and reply -->
  <div>
    <!-- v-model is holding value from what we type in input field and binds that to data() -->
    <!-- On ctrl + enter, emit sendComment event with content, clearable => spawns X to the right -->
    <v-text-field label="Comment"
                  v-model="content"
                  clearable
                  @keydown.ctrl.enter="send">
    </v-text-field>

    <!-- Emitting cancel event, $emit() function allows us to pass custom event/s up the component tree -->
    <!-- We use $emit() to notify the parent component that something changed. -->
    <!-- * Dont show cancel button if we are not hooking to that event somewhere -->
    <v-btn v-if="$listeners['cancel']" @click="cancel">Cancel</v-btn>

    <!-- Emitting sendComment event, with comment content, when we hook to this event we can grab content value,
    dynamically passing label which is coming from "comment-input label="comment"" in comment-section,
    when we press "Comment" content get's passed all the way to modItem page and then we create comment for that mod item -->
    <!-- Button is disabled only when we dont have content (we didnt typed anything) -->
    <v-btn :disabled="!content" @click="send">{{label}}</v-btn>
  </div>
</template>

<script>
import {configurable, creator} from "@/components/comments/_shared";

  export default {
    // Component name
    name: "comment-input",

    mixins: [creator, configurable],

    // Component props
    props: {
      // Label
      label: {
        required: false,
        type: String,
        default: "send"
      }
    },

    // Component local state
    data: () => ({
      // Comment's content
      content: ""
    }),

    methods: {
      send() {
        const data = {
          parentId: this.parentId,
          parentType: this.parentType,
          content: this.content
        }

        return this.$axios.$post('/api/comments', data)
          .then(this.emitComment)
          .then(this.cancel)
      }
    }

  }
</script>

<style scoped>

</style>
