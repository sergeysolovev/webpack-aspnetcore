import './polyfills/promise';
import './index.less';

const hasAnyClasses = (...args) => args.some(
  cls => document.getElementsByClassName(cls).length
);

window.addEventListener('load', function () {
  if (hasAnyClasses('carousel')) {
    import(/* webpackChunkName: 'carousel' */ './components/carousel')
      .catch(error => console.error(error));
  }
});
