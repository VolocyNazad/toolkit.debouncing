Debounce and throttle now throw `ArgumentNullException` immediately when the callback is `null`, instead of failing later on the dispatcher thread.
