import React from 'react';

const ExampleVideoPage: React.FC = () => {
  return (
    <div>
      <h2>Example Video</h2>
      <iframe
        width="560"
        height="315"
        src="https://www.youtube.com/embed/dQw4w9WgXcQ?si=0bqX-Yo5mGDloO23" // Placeholder video ID
        title="YouTube video player"
        frameBorder="0"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
        allowFullScreen
      ></iframe>
    </div>
  );
};

export default ExampleVideoPage;