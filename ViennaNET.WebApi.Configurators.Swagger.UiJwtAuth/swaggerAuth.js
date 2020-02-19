function keyPress(key) {
  var event = document.createEvent('Event');
  event.keyCode = key; // Deprecated, prefer .key instead.
  event.key = key;
  event.initEvent('keydown');

  return event;
}

function waitForElement() {
  return new Promise(function (resolve, reject) {
    var observer = new MutationObserver(function (mutations) {
      mutations.forEach(function (mutation) {
        const nodes = Array.from(mutation.addedNodes);
        for (let node of nodes) {
          if (node.className
            && node.className === "scheme-container"
            && node.innerHTML
            && node.innerHTML.includes("btn authorize unlocked")) {
            observer.disconnect();
            resolve(node);
            return;
          }
        };
      });
    });

    observer.observe(document.documentElement, { childList: true, subtree: true });
  });
}

const enterAuthToken = (token) => {
  document.getElementsByClassName("btn authorize")[0].click();
  const modalContainer = document.getElementsByClassName("auth-container")[0];
  const input = modalContainer.getElementsByTagName("input")[0];
  input.focus();

  let lastValue = input.value;
  input.value = token;
  let event = new Event('input', { bubbles: true });
  // hack React15
  event.simulated = true;
  // hack React16
  let tracker = input._valueTracker;
  if (tracker) {
    tracker.setValue(lastValue);
  }
  input.dispatchEvent(event);

  modalContainer.getElementsByClassName("btn modal-btn auth authorize button")[0].click();
  modalContainer.getElementsByClassName("btn modal-btn auth btn-done button")[0].click();
}

waitForElement().then(() => {
  fetch('<security-host>/api/tokens',
    {
      cache: 'no-cache',
      credentials: 'include',
      method: '<request-method>'
    }).then(response => {
      return response.json();
    }).then(authInfo => {
      enterAuthToken(authInfo.token);
    });
}).catch(e => console.log(e));
