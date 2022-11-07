<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">

[//]: # (  <a href="https://github.com/svanderweele/core-api">)

[//]: # (    <img src="images/logo.png" alt="Logo" width="80" height="80">)

[//]: # (  </a>)

<h3 align="center">Core Technical Test for Senior Developer</h3>

  <p align="center">
    A technical application requested by the core team to determine my skill level as a developer.
    <br />
    <a href="https://github.com/svanderweele/core-api"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="http://dev-core-game-bucket.s3-website-eu-west-1.amazonaws.com">View Demo</a>
    ·
    <a href="https://github.com/svanderweele/core-api/issues">Report Bug</a>
    ·
    <a href="https://github.com/svanderweele/core-api/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



### Built With

* .NET 6
<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

A simple application built to become more familiar with .NET 6 as well as best practices.
### Prerequisites

* Redis Server to run application locally. Cannot connect to remote elasticache instance due to it being behind a private network

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/svanderweele/core-api.git
   ```
2. Run either the Gaming.API or the Authentication.API

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- ROADMAP -->
## Roadmap

- [x] Add dynamic configuration files
  - [x] Consider something like Consul
  - [x] Dynamic Config handled through [AWS Parameter Store](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-parameter-store.html)
- [x] Terraform Refactoring
- [x] Auth Token Validation should be improved
- [x] AWS VPC needs to be redesigned to strengthen security for AWS resources (mainly lambda & codebuild permissions)
- [x] Resolve issue with CodeBuild pipeline
- [x] FE side to it. A simple web app hosted on AWS s3 (built in either next or angular) working with the API [WIP](http://dev-core-game-bucket.s3-website-eu-west-1.amazonaws.com/)
- [ ] [FE] Better Error handling using HTTP Interceptors & Passing Auth Token in the same manner
- [ ] Consider a different approach to data handling with DynamoDB

See the [open issues](https://github.com/svanderweele/core-api/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>




<!-- CONTACT -->
## Contact

Simon van der Weele - vanderweelesimon@gmail.com

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/svanderweele/core-api.svg?style=for-the-badge
[contributors-url]: https://github.com/svanderweele/core-api/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/svanderweele/core-api.svg?style=for-the-badge
[forks-url]: https://github.com/svanderweele/core-api/network/members
[stars-shield]: https://img.shields.io/github/stars/svanderweele/core-api.svg?style=for-the-badge
[stars-url]: https://github.com/svanderweele/core-api/stargazers
[issues-shield]: https://img.shields.io/github/issues/svanderweele/core-api.svg?style=for-the-badge
[issues-url]: https://github.com/svanderweele/core-api/issues
[license-shield]: https://img.shields.io/github/license/svanderweele/core-api.svg?style=for-the-badge
[license-url]: https://github.com/svanderweele/core-api/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/simon-van-der-weele-69b28896
[product-screenshot]: images/screenshot.png
[Next.js]: https://img.shields.io/badge/next.js-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://nextjs.org/
[React.js]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[React-url]: https://reactjs.org/
[Vue.js]: https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D
[Vue-url]: https://vuejs.org/
[Angular.io]: https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white
[Angular-url]: https://angular.io/
[Svelte.dev]: https://img.shields.io/badge/Svelte-4A4A55?style=for-the-badge&logo=svelte&logoColor=FF3E00
[Svelte-url]: https://svelte.dev/
[Laravel.com]: https://img.shields.io/badge/Laravel-FF2D20?style=for-the-badge&logo=laravel&logoColor=white
[Laravel-url]: https://laravel.com
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[JQuery.com]: https://img.shields.io/badge/jQuery-0769AD?style=for-the-badge&logo=jquery&logoColor=white
[JQuery-url]: https://jquery.com 
