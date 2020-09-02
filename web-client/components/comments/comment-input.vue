<template>
  <!-- Comment input, place where we type in our comment, reusable for comment and reply -->
  <div>
    <!-- v-model is holding value from what we type in input field and binds that to data() -->
    <v-text-field label="Comment" v-model="content"></v-text-field>

    <!-- Emitting cancel event, $emit() function allows us to pass custom event/s up the component tree -->
    <!-- We use $emit() to notify the parent component that something changed. -->
    <!-- * Dont show cancel button if we are not hooking to that event somewhere -->
    <v-btn v-if="$listeners['cancel']" @click="$emit('cancel')">Cancel</v-btn>

    <!-- Emitting send event, with comment content, when we hook to this event we can grab content value,
    dynamically passing label which is coming from "comment-input label="comment"" in comment-section,
    when we press "Comment" content get's passed all the way to modItem page and then we create comment for that mod item -->
    <v-btn @click="$emit('send', content)">{{label}}</v-btn>
  </div>
</template>

<script>
  export default {
    // Component name
    name: "comment-input",

    // Component props
    props: {
      // Label
      label: {
        required: false,
        type: String,
        default: "reply"
      }
    },

    // Component local state
    data: () => ({
      // Comment's content
      content: ""
    }),

  }
</script>

<style scoped>

</style>
