// <copyright file="ShadowQuadHelper.cs" company="Google LLC">
//
// Copyright 2020 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// Helper class to activate/deactivate the light estimation shadow plane.
    /// </summary>
    public class ShadowQuadHelper : MonoBehaviour
    {
        /// <summary>
        /// The Depth Setting Menu.
        /// </summary>
        private DepthMenu m_DepthMenu;

        /// <summary>
        /// The GameObject of ShadowQuad.
        /// </summary>
        private GameObject m_ShadowQuad;

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            m_ShadowQuad = this.gameObject.transform.Find("ShadowQuad").gameObject;
            m_DepthMenu = FindObjectOfType<DepthMenu>();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            // Shadows are cast onto the light estimation shadow plane, which do not respect depth.
            // Shadows are disabled when depth is enabled to prevent undesirable rendering
            // artifacts.
            if (m_ShadowQuad.activeSelf == m_DepthMenu.IsDepthEnabled())
            {
                m_ShadowQuad.SetActive(!m_DepthMenu.IsDepthEnabled());
            }
        }
    }
}
