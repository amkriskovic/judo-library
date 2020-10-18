<template>
  <div>
    <v-row>
      <!-- Content on the left -->
      <v-col cols="12" sm="7" lg="6" offset-lg="1" xl="4" offset-xl="3">
        <!-- List of techniques/submissions(video) -->
        <slot name="content"></slot>
      </v-col>

      <!-- Content of the right(card) information -->
      <v-col class="d-none d-sm-block" sm="5" lg="4" xl="3">
        <!-- v-sheet represents "card" with information -->
        <v-sheet class="pa-3 sticky">
          <!-- Binding close (: -> dynamic) attribute to some function -->
          <slot name="item" :close="() => {}"></slot>
        </v-sheet>
      </v-col>
    </v-row>

    <!-- * Dialog -> Popup -> onclick(when is opened) sets dialog to true, only visible on MOBILE -->
    <v-dialog v-model="dialog">

      <template v-slot:activator="{on}">
        <!-- Button, show button that show's card only on small sized screens -->
        <v-btn class="d-sm-none" fixed fab bottom right small color="blue" v-on="on">
          <v-icon>mdi-help-circle-outline</v-icon>
        </v-btn>
      </template>

      <!-- Inject modItem(card) slot -->
      <!-- v-sheet represents "card" with information -->
      <v-sheet class="pa-3" rounded>
        <!-- Binding close (: -> dynamic) attribute to some function => setting dialog to false -->
        <slot name="item" :close="() => dialog = false"></slot>
      </v-sheet>

    </v-dialog>
  </div>
</template>

<script>
  // Component name
  export default {
    name: "item-content-layout",

    // Component local state
    data: () => ({
      dialog: false
    })
  }
</script>

<style scoped>

</style>
