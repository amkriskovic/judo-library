<template>
  <div>
    <!-- * Comment body component is just about how do we display that particular comment, display htmlContent of comment -->
    <div class="my-1">
      <!-- Comment that we are replying to -->
      <p class="mb-1" v-html="comment.htmlContent"></p>

      <!-- This is where we wanna spawn input field for replying to comment, onClick setting replying to true -->
      <!-- replying toggle, if we click reply input field for replying =>> comment-input -->
      <v-btn small text v-if="!replying" @click="replying = true">Reply</v-btn>

      <!-- * Dont show load-replies button if we are not hooking to that event somewhere -->
      <v-btn small text v-if="$listeners['load-replies']" @click="$emit('load-replies')">Show Replies</v-btn>
    </div>

    <!-- Injecting comment-input component, emitting send event with content -> that reply holds -->
    <!-- @cancel event, we set replying to false => means we spawn cancel button in order to cancel reply if we want to -->
    <!-- if we are replying show the bar -->
    <comment-input v-if="replying" @send="(content) => $emit('send', content)" @cancel="replying = false"/>
  </div>
</template>

<script>
  import CommentInput from "./comment-input";

  export default {
    // Component name
    name: "comment-body",

    // Injected components
    components: {CommentInput},

    // Component props
    props: {
      // Comment
      comment: {
        required: true,
        type: Object,
      }
    },

    // Component local state
    data: () => ({
      replying: false
    }),
  }
</script>

<style scoped>

</style>
