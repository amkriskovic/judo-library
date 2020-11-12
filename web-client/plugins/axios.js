export default function({$axios}) {
  $axios.setHeader('X-Requested-With', 'XMLHttpRequest')

  $axios.onRequest(config => {
    // For every req. add cookie
    config.withCredentials = true
  })
}
